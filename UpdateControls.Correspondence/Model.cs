using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence
{
    class Model
    {
        public const short MaxDataLength = 1024;

        private const long ClientDatabaseId = 0;

		private Community _community;
		private IStorageStrategy _storageStrategy;
        private ITypeStrategy _typeStrategy;
        private IDictionary<CorrespondenceFactType, ICorrespondenceFactFactory> _factoryByType =
            new Dictionary<CorrespondenceFactType, ICorrespondenceFactFactory>();
        private IDictionary<CorrespondenceFactType, List<QueryInvalidator>> _queryInvalidatorsByType =
            new Dictionary<CorrespondenceFactType, List<QueryInvalidator>>();
        private IDictionary<CorrespondenceFactType, FactMetadata> _metadataByFactType = new Dictionary<CorrespondenceFactType, FactMetadata>();

        private IDictionary<FactID, CorrespondenceFact> _factByID = new Dictionary<FactID, CorrespondenceFact>();
		private IDictionary<FactMemento, CorrespondenceFact> _factByMemento = new Dictionary<FactMemento, CorrespondenceFact>();

        public event Action FactAdded;

        public Model(Community community, IStorageStrategy storageStrategy, ITypeStrategy typeStrategy)
		{
            _community = community;
			_storageStrategy = storageStrategy;
            _typeStrategy = typeStrategy;
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

        public IDisposable BeginDuration()
        {
            return _storageStrategy.BeginDuration();
        }

        public T AddFact<T>(T prototype) where T : CorrespondenceFact
        {
            return AddFact<T>(prototype, 0);
        }

        private T AddFact<T>(T prototype, int peerId) where T : CorrespondenceFact
        {
            // Save invalidate actions until after the lock
            // because they reenter if a fact is removed.
            List<Action> invalidateActions = new List<Action>();

            lock (this)
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

                // Set the ID and add the object to the community.
                FactID id;
                if (_storageStrategy.Save(memento, peerId, out id))
                {
                    // Invalidate all of the queries affected by the new object.
                    List<QueryInvalidator> invalidators;
                    foreach (CorrespondenceFactType factType in _metadataByFactType[memento.FactType].ConvertableTypes)
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
                                    targetFactIds = _storageStrategy.QueryForIds(invalidator.TargetFacts, id).ToList();

                                foreach (FactID targetObjectId in targetFactIds)
                                {
                                    CorrespondenceFact targetObject;
                                    if (_factByID.TryGetValue(targetObjectId, out targetObject))
                                    {
                                        QueryDefinition invalidQuery = invalidator.InvalidQuery;
                                        invalidateActions.Add(() => targetObject.InvalidateQuery(invalidQuery));
                                    }
                                }
                            }
                        }
                }

                // Turn the prototype into the real fact.
                prototype.ID = id;
                prototype.SetCommunity(_community);
                _factByID.Add(prototype.ID, prototype);
                _factByMemento.Add(memento, prototype);
            }

            foreach (Action invalidateAction in invalidateActions)
                invalidateAction();

            if (FactAdded != null)
                FactAdded();

            return prototype;
        }

        public T FindFact<T>(T prototype)
                where T : CorrespondenceFact
        {
            lock (this)
            {
                if (prototype.InternalCommunity != null)
                    throw new CorrespondenceException("A fact may only belong to one community");

                FactMemento memento = CreateMementoFromFact(prototype);

                // See if we already have the fact in memory.
                CorrespondenceFact fact;
                if (_factByMemento.TryGetValue(memento, out fact))
                    return (T)fact;

                // If the object is alredy in storage, load it.
                FactID id;
                if (_storageStrategy.FindExistingFact(memento, out id))
                {
                    prototype.ID = id;
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

        public CorrespondenceFact LoadFact(string factName)
        {
            FactID id;
            if (_storageStrategy.GetID(factName, out id))
                return GetFactByID(id);
            else
                return null;
        }

        public void SetFact(string factName, CorrespondenceFact obj)
        {
            _storageStrategy.SetID(factName, obj.ID);
        }

        public FactTreeMemento GetMessageBodies(ref TimestampID timestamp, int peerId)
		{
			FactTreeMemento result = new FactTreeMemento(ClientDatabaseId);
            IEnumerable<MessageMemento> recentMessages = _storageStrategy.LoadRecentMessagesForServer(peerId, timestamp);
			foreach (MessageMemento message in recentMessages)
			{
				if (message.FactId.key > timestamp.Key)
					timestamp = new TimestampID(ClientDatabaseId, message.FactId.key);
				AddToFactTree(result, message.FactId);
			}
			return result;
		}


        public void AddToFactTree(FactTreeMemento messageBody, FactID factId)
        {
            if (!messageBody.Contains(factId))
            {
                CorrespondenceFact fact = GetFactByID(factId);
                if (fact != null)
                {
                    FactMemento factMemento = CreateMementoFromFact(fact);
                    foreach (PredecessorMemento predecessor in factMemento.Predecessors)
                        AddToFactTree(messageBody, predecessor.ID);
                    messageBody.Add(new IdentifiedFactMemento(factId, factMemento));
                }
            }
        }

        public void ReceiveMessage(FactTreeMemento messageBody, int peerId)
        {
            IDictionary<FactID, FactID> localIdByRemoteId = new Dictionary<FactID, FactID>();
            foreach (IdentifiedFactMemento identifiedFact in messageBody.Facts)
            {
                FactMemento translatedMemento = new FactMemento(identifiedFact.Memento.FactType);
                translatedMemento.Data = identifiedFact.Memento.Data;
                translatedMemento.AddPredecessors(identifiedFact.Memento.Predecessors
                    .Select(remote =>
                    {
                        FactID localFactId;
                        return !localIdByRemoteId.TryGetValue(remote.ID, out localFactId)
                            ? null
                            : new PredecessorMemento(remote.Role, localFactId);
                    })
                    .Where(pred => pred != null));

                try
                {
                    CorrespondenceFact fact = HydrateFact(translatedMemento);
                    fact = AddFact(fact, peerId);
                    FactID localId = fact.ID;
                    FactID remoteId = identifiedFact.Id;
                    localIdByRemoteId.Add(remoteId, localId);
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        public CorrespondenceFact GetFactByID(FactID id)
        {
            // Check for null.
            if (id.key == 0)
                return null;

            lock (this)
            {
                // See if the object is already loaded.
                CorrespondenceFact obj;
                if (_factByID.TryGetValue(id, out obj))
                    return obj;

                // If not, load it from storage.
                FactMemento memento = _storageStrategy.Load(id);
                return CreateFactFromMemento(id, memento);
            }
        }

		public Guid ClientDatabaseGuid
		{
			get { return _storageStrategy.ClientGuid; }
		}

        internal IEnumerable<CorrespondenceFact> ExecuteQuery(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            lock (this)
            {
                List<IdentifiedFactMemento> facts = _storageStrategy.QueryForFacts(queryDefinition, startingId, options).ToList();
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
            CorrespondenceFactType type = _typeStrategy.GetTypeOfFact(prototype);

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
                    memento.AddPredecessor(role, id);
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

        private void HandleException(Exception exception)
        {
            // TODO: Find some way of notifying the application developer without interrupting the process.
        }
    }
}
