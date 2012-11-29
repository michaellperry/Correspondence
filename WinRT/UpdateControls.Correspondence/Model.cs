using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Conditions;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence
{
    class Model
    {
        public const short MaxDataLength = 1024;

        private const long ClientDatabaseId = 0;

		private Community _community;
		private IStorageStrategy _storageStrategy;

        private bool _clientApp = true;
        private IDictionary<CorrespondenceFactType, ICorrespondenceFactFactory> _factoryByType =
            new Dictionary<CorrespondenceFactType, ICorrespondenceFactFactory>();
        private IDictionary<CorrespondenceFactType, List<QueryInvalidator>> _queryInvalidatorsByType =
            new Dictionary<CorrespondenceFactType, List<QueryInvalidator>>();
        private IDictionary<CorrespondenceFactType, List<Unpublisher>> _unpublishersByType =
            new Dictionary<CorrespondenceFactType, List<Unpublisher>>();
        private IDictionary<CorrespondenceFactType, FactMetadata> _metadataByFactType = new Dictionary<CorrespondenceFactType, FactMetadata>();

        private IDictionary<FactID, CorrespondenceFact> _factByID = new Dictionary<FactID, CorrespondenceFact>();
		private IDictionary<FactMemento, CorrespondenceFact> _factByMemento = new Dictionary<FactMemento, CorrespondenceFact>();

        public event Action<CorrespondenceFact> FactAdded;
        public event Action FactReceived;

        public AwaitableCriticalSection _lock = new AwaitableCriticalSection();

        public Model(Community community, IStorageStrategy storageStrategy)
		{
            _community = community;
			_storageStrategy = storageStrategy;
		}

        public bool ClientApp
        {
            get { return _clientApp; }
            set { _clientApp = value; }
        }

        public void AddType(CorrespondenceFactType type, ICorrespondenceFactFactory factory, FactMetadata factMetadata)
        {
            _factoryByType[type] = factory;
            _metadataByFactType.Add(type, factMetadata);
        }

        public void AddQuery(CorrespondenceFactType type, QueryDefinition query)
        {
            // Invert the query.
            new QueryInverter(type, delegate(CorrespondenceFactType targetType, QueryDefinition inverse)
                {
                    // Record the inverse as a query invalidator by the target type.
                    List<QueryInvalidator> invalidators;
                    if (!_queryInvalidatorsByType.TryGetValue(targetType, out invalidators))
                    {
                        invalidators = new List<QueryInvalidator>();
                        _queryInvalidatorsByType.Add(targetType, invalidators);
                    }
                    invalidators.Add(new QueryInvalidator(inverse, query));
                })
                .InvertQuery(query.Joins);
        }

        public void AddUnpublisher(RoleMemento role, Condition publishCondition)
        {
            // Invert the query.
            var queryInverter = new QueryInverter(role.DeclaringType, delegate(CorrespondenceFactType targetType, QueryDefinition inverse)
            {
                // Record the inverse as an unpublisher by the target type.
                List<Unpublisher> unpublishers;
                if (!_unpublishersByType.TryGetValue(targetType, out unpublishers))
                {
                    unpublishers = new List<Unpublisher>();
                    _unpublishersByType.Add(targetType, unpublishers);
                }
                unpublishers.Add(new Unpublisher(inverse, publishCondition, role));
            });
            queryInverter.HandleCondition(publishCondition);
        }

        public async Task<T> AddFactAsync<T>(T prototype) where T : CorrespondenceFact
        {
            return await AddFactAsync<T>(prototype, 0);
        }

        private async Task<T> AddFactAsync<T>(T prototype, int peerId) where T : CorrespondenceFact
        {
            // Save invalidate actions until after the lock
            // because they reenter if a fact is removed.
            Dictionary<InvalidatedQuery, InvalidatedQuery> invalidatedQueries = new Dictionary<InvalidatedQuery, InvalidatedQuery>();

            using (await _lock.EnterAsync())
            {
                if (prototype.InternalCommunity != null)
                    throw new CorrespondenceException("A fact may only belong to one community");

                foreach (RoleMemento role in prototype.PredecessorRoles)
                {
                    PredecessorBase predecessor = prototype.GetPredecessor(role);
                    if (predecessor.Community != null && predecessor.Community != _community)
                        throw new CorrespondenceException("A fact cannot be added to a different community than its predecessors.");
                }

                FactMemento memento = CreateMementoFromFact(prototype);

                // See if we already have the fact in memory.
                CorrespondenceFact fact;
                if (_factByMemento.TryGetValue(memento, out fact))
                    return (T)fact;

                FactID id = await AddFactMementoAsync(peerId, memento, invalidatedQueries);

                // Turn the prototype into the real fact.
                prototype.ID = id;
                prototype.SetCommunity(_community);
                _factByID.Add(prototype.ID, prototype);
                _factByMemento.Add(memento, prototype);
            }

            foreach (InvalidatedQuery invalidatedQuery in invalidatedQueries.Keys)
                invalidatedQuery.Invalidate();

            if (FactAdded != null)
                FactAdded(prototype);

            return prototype;
        }

        public async Task<T> FindFactAsync<T>(T prototype)
                where T : CorrespondenceFact
        {
            using (await _lock.EnterAsync())
            {
                if (prototype.InternalCommunity != null)
                    throw new CorrespondenceException("A fact may only belong to one community");

                FactMemento memento = CreateMementoFromFact(prototype);

                // See if we already have the fact in memory.
                CorrespondenceFact fact;
                if (_factByMemento.TryGetValue(memento, out fact))
                    return (T)fact;

                // If the object is alredy in storage, load it.
                FactID? existingFactId = await _storageStrategy.FindExistingFactAsync(memento);
                if (existingFactId.HasValue)
                {
                    prototype.ID = existingFactId.Value;
                    prototype.SetCommunity(_community);
                    _factByID.Add(prototype.ID, prototype);
                    _factByMemento.Add(memento, prototype);
                    return prototype;
                }
                else
                {
                    // The object does not exist.
                    return null;
                }
            }
        }

        public async Task<CorrespondenceFact> LoadFactAsync(string factName)
        {
            FactID? id = await _storageStrategy.GetIDAsync(factName);
            if (id.HasValue)
                return await GetFactByIDAsync(id.Value);
            else
                return null;
        }

        public async Task SetFactAsync(string factName, CorrespondenceFact obj)
        {
            await _storageStrategy.SetIDAsync(factName, obj.ID);
        }

        public async Task<GetResultMemento> GetMessageBodiesAsync(TimestampID timestamp, int peerId, List<UnpublishMemento> unpublishedMessages)
        {
            FactTreeMemento result = new FactTreeMemento(ClientDatabaseId);
            IEnumerable<MessageMemento> recentMessages = await _storageStrategy.LoadRecentMessagesForServerAsync(peerId, timestamp);
            foreach (MessageMemento message in recentMessages)
            {
                if (message.FactId.key > timestamp.Key)
                    timestamp = new TimestampID(ClientDatabaseId, message.FactId.key);
                FactMemento newFact = await AddToFactTreeAsync(result, message.FactId, peerId);

                FactMetadata factMetadata;
                if (newFact != null && _metadataByFactType.TryGetValue(newFact.FactType, out factMetadata))
                {
                    IEnumerable<CorrespondenceFactType> convertableTypes = factMetadata.ConvertableTypes;
                    foreach (CorrespondenceFactType convertableType in convertableTypes)
                    {
                        List<Unpublisher> unpublishers;
                        if (_unpublishersByType.TryGetValue(convertableType, out unpublishers))
                        {
                            foreach (Unpublisher unpublisher in unpublishers)
                            {
                                IEnumerable<FactID> messageIds = await _storageStrategy.QueryForIdsAsync(unpublisher.MessageFacts, message.FactId);
                                foreach (FactID messageId in messageIds)
                                {
                                    ConditionEvaluator conditionEvaluator = new ConditionEvaluator(_storageStrategy);
                                    bool published = await conditionEvaluator.EvaluateAsync(messageId, unpublisher.PublishCondition);
                                    if (!published)
                                    {
                                        await AddToFactTreeAsync(result, messageId, peerId);
                                        UnpublishMemento unpublishMemento = new UnpublishMemento(messageId, unpublisher.Role);
                                        if (!unpublishedMessages.Contains(unpublishMemento))
                                            unpublishedMessages.Add(unpublishMemento);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new GetResultMemento(result, timestamp);
        }


        public async Task<FactMemento> AddToFactTreeAsync(FactTreeMemento messageBody, FactID factId, int peerId)
        {
            if (!messageBody.Contains(factId))
            {
                CorrespondenceFact fact = await GetFactByIDAsync(factId);
                if (fact != null)
                {
                    FactID? remoteId = await _storageStrategy.GetRemoteIdAsync(factId, peerId);
                    if (remoteId.HasValue)
                    {
                        messageBody.Add(new IdentifiedFactRemote(factId, remoteId.Value));
                    }
                    else
                    {
                        FactMemento factMemento = CreateMementoFromFact(fact);
                        foreach (PredecessorMemento predecessor in factMemento.Predecessors)
                            await AddToFactTreeAsync(messageBody, predecessor.ID, peerId);
                        messageBody.Add(new IdentifiedFactMemento(factId, factMemento));

                        return factMemento;
                    }
                }
                return null;
            }
            else
            {
                IdentifiedFactBase identifiedFact = messageBody.Get(factId);
                if (identifiedFact is IdentifiedFactMemento)
                    return ((IdentifiedFactMemento)identifiedFact).Memento;
                else
                    return null;
            }
        }

        public async void ReceiveMessage(FactTreeMemento messageBody, int peerId)
        {
            Dictionary<InvalidatedQuery, InvalidatedQuery> invalidatedQueries = new Dictionary<InvalidatedQuery, InvalidatedQuery>();

            using (await _lock.EnterAsync())
            {
                IDictionary<FactID, FactID> localIdByRemoteId = new Dictionary<FactID, FactID>();
                foreach (IdentifiedFactBase identifiedFact in messageBody.Facts)
                {
                    FactID localId = await ReceiveFactAsync(identifiedFact, peerId, invalidatedQueries, localIdByRemoteId);
                    FactID remoteId = identifiedFact.Id;
                    localIdByRemoteId.Add(remoteId, localId);
                }
            }

            foreach (InvalidatedQuery invalidatedQuery in invalidatedQueries.Keys)
                invalidatedQuery.Invalidate();
        }

        private async Task<FactID> ReceiveFactAsync(IdentifiedFactBase identifiedFact, int peerId, Dictionary<InvalidatedQuery, InvalidatedQuery> invalidatedQueries, IDictionary<FactID, FactID> localIdByRemoteId)
        {
            if (identifiedFact is IdentifiedFactMemento)
            {
                IdentifiedFactMemento identifiedFactMemento = (IdentifiedFactMemento)identifiedFact;
                FactMemento translatedMemento = new FactMemento(identifiedFactMemento.Memento.FactType);
                translatedMemento.Data = identifiedFactMemento.Memento.Data;
                translatedMemento.AddPredecessors(identifiedFactMemento.Memento.Predecessors
                    .Select(remote =>
                    {
                        FactID localFactId;
                        return !localIdByRemoteId.TryGetValue(remote.ID, out localFactId)
                            ? null
                            : new PredecessorMemento(remote.Role, localFactId, remote.IsPivot);
                    })
                    .Where(pred => pred != null));

                FactID localId = await AddFactMementoAsync(peerId, translatedMemento, invalidatedQueries);
                await _storageStrategy.SaveShareAsync(peerId, identifiedFact.Id, localId);

                return localId;
            }
            else
            {
                IdentifiedFactRemote identifiedFactRemote = (IdentifiedFactRemote)identifiedFact;
                return await _storageStrategy.GetFactIDFromShareAsync(peerId, identifiedFactRemote.RemoteId);
            }
        }

        public async Task<CorrespondenceFact> GetFactByIDAsync(FactID id)
        {
            // Check for null.
            if (id.key == 0)
                return null;

            using (await _lock.EnterAsync())
            {
                // See if the object is already loaded.
                CorrespondenceFact obj;
                if (_factByID.TryGetValue(id, out obj))
                    return obj;

                // If not, load it from storage.
                FactMemento memento = await _storageStrategy.LoadAsync(id);
                return CreateFactFromMemento(id, memento);
            }
        }

        public async Task<Guid> GetClientDatabaseGuidAsync()
        {
            return await _storageStrategy.GetClientGuidAsync();
        }

        internal async Task<List<CorrespondenceFact>> ExecuteQueryAsync(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            List<IdentifiedFactMemento> facts = await _storageStrategy.QueryForFactsAsync(queryDefinition, startingId, options);
            using (await _lock.EnterAsync())
            {
                return facts
                    .Select(m => GetFactByIdAndMemento(m.Id, m.Memento))
                    .Where(m => m != null)
                    .ToList();
            }
        }

        private CorrespondenceFact GetFactByIdAndMemento(FactID id, FactMemento memento)
        {
            // Check for null.
            if (id.key == 0)
                return null;

            // See if the fact is already loaded.
            CorrespondenceFact fact;
            if (_factByID.TryGetValue(id, out fact))
                return fact;

            // If not, create it from the memento.
            return CreateFactFromMemento(id, memento);
        }

        private CorrespondenceFact CreateFactFromMemento(FactID id, FactMemento memento)
        {
            try
            {
                System.Diagnostics.Debug.Assert(
                    !_factByMemento.ContainsKey(memento),
                    "A memento already in memory is being reloaded");

                // Invoke the factory to create the object.
                if (memento == null)
                    throw new CorrespondenceException("Failed to load fact");

                CorrespondenceFact fact = HydrateFact(memento);

                fact.ID = id;
                fact.SetCommunity(_community);
                _factByID.Add(fact.ID, fact);
                _factByMemento.Add(memento, fact);
                return fact;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }
        }

        private CorrespondenceFact HydrateFact(FactMemento memento)
        {
            ICorrespondenceFactFactory factory;
            if (!_factoryByType.TryGetValue(memento.FactType, out factory))
                throw new CorrespondenceException(string.Format("Unknown type: {0}", memento.FactType));

            CorrespondenceFact fact = factory.CreateFact(memento);
            if (fact == null)
                throw new CorrespondenceException("Failed to create fact");
            return fact;
        }

        private FactMemento CreateMementoFromFact(CorrespondenceFact prototype)
        {
            // Get the type of the object.
            CorrespondenceFactType type = prototype.GetCorrespondenceFactType();

            // Find the factory for that type.
            ICorrespondenceFactFactory factory;
            if (!_factoryByType.TryGetValue(type, out factory))
                throw new CorrespondenceException(string.Format("Add the type {0} or the assembly that contains it to the community.", type));

            // Create the memento.
            FactMemento memento = new FactMemento(type);
            foreach (RoleMemento role in prototype.PredecessorRoles)
            {
                PredecessorBase predecessor = prototype.GetPredecessor(role);
                foreach (FactID id in predecessor.InternalFactIds)
                    memento.AddPredecessor(role, id, role.IsPivot);
            }
            using (MemoryStream data = new MemoryStream())
            {
                using (BinaryWriter output = new BinaryWriter(data))
                {
                    factory.WriteFactData(prototype, output);
                }
                memento.Data = data.ToArray();
                if (memento.Data.Length > MaxDataLength)
                    throw new CorrespondenceException(string.Format("Fact data length {0} exceeded the maximum bytes allowable ({1}).", memento.Data.Length, Model.MaxDataLength));
            }

            return memento;
        }

        private async Task<FactID> AddFactMementoAsync(int peerId, FactMemento memento, Dictionary<InvalidatedQuery, InvalidatedQuery> invalidatedQueries)
        {
            // Set the ID and add the object to the community.
            var result = await _storageStrategy.SaveAsync(memento, peerId);
            FactID id = result.Id;
            if (result.WasSaved)
            {
                // Invalidate all of the queries affected by the new object.
                List<QueryInvalidator> invalidators;
                FactMetadata metadata;
                if (_metadataByFactType.TryGetValue(memento.FactType, out metadata))
                {
                    foreach (CorrespondenceFactType factType in metadata.ConvertableTypes)
                        if (_queryInvalidatorsByType.TryGetValue(factType, out invalidators))
                        {
                            foreach (QueryInvalidator invalidator in invalidators)
                            {
                                List<FactID> targetFactIds;
                                if (invalidator.TargetFacts.CanExecuteFromMemento)
                                    // If the inverse query is simple, the results are already in the memento.
                                    targetFactIds = invalidator.TargetFacts.ExecuteFromMemento(memento);
                                else
                                    // The query is more complex, so go to the storage strategy.
                                    targetFactIds = (await _storageStrategy.QueryForIdsAsync(invalidator.TargetFacts, id)).ToList();

                                foreach (FactID targetObjectId in targetFactIds)
                                {
                                    CorrespondenceFact targetObject;
                                    if (_factByID.TryGetValue(targetObjectId, out targetObject))
                                    {
                                        QueryDefinition invalidQuery = invalidator.InvalidQuery;
                                        InvalidatedQuery query = new InvalidatedQuery(targetObject, invalidQuery);
                                        if (!invalidatedQueries.ContainsKey(query))
                                            invalidatedQueries.Add(query, query);
                                    }
                                }
                            }
                        }
                }

                if (FactReceived != null)
                    FactReceived();
            }
            return id;
        }

        private void HandleException(Exception exception)
        {
            // TODO: Find some way of notifying the application developer without interrupting the process.
        }
    }
}
