using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence
{
    class Model : IMessageRepository
    {
        private Community _community;
		private IStorageStrategy _storageStrategy;
        private ITypeStrategy _typeStrategy;
        private IDictionary<CorrespondenceFactType, ICorrespondenceFactFactory> _factoryByType =
            new Dictionary<CorrespondenceFactType, ICorrespondenceFactFactory>();
        private IDictionary<CorrespondenceFactType, List<QueryInvalidator>> _queryInvalidatorsByType =
            new Dictionary<CorrespondenceFactType, List<QueryInvalidator>>();

        private IDictionary<FactID, CorrespondenceFact> _factByID = new Dictionary<FactID, CorrespondenceFact>();
		private IDictionary<FactMemento, CorrespondenceFact> _factByMemento = new Dictionary<FactMemento, CorrespondenceFact>();

        public Model(Community community, IStorageStrategy storageStrategy, ITypeStrategy typeStrategy)
		{
            _community = community;
			_storageStrategy = storageStrategy;
            _typeStrategy = typeStrategy;
		}

        public void AddType(CorrespondenceFactType type, ICorrespondenceFactFactory factory)
        {
            _factoryByType[type] = factory;
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

        public IDisposable BeginUnitOfWork()
        {
            return _storageStrategy.BeginUnitOfWork();
        }

        public T AddFact<T>(T prototype) where T : CorrespondenceFact
        {
            // Save invalidate actions until after the lock
            // because they reenter if a fact is removed.
            List<Action> invalidateActions = new List<Action>();

            lock (this)
            {
                if (prototype.Community != null)
                    throw new CorrespondenceException("A fact may only belong to one community");

                FactMemento memento = CreateMementoFromFact(prototype);

                // See if we already have the fact in memory.
                CorrespondenceFact fact;
                if (_factByMemento.TryGetValue(memento, out fact))
                    return (T)fact;

                // Set the ID and add the object to the community.
                FactID id;
                bool saved = _storageStrategy.Save(memento, FindPivots(prototype).ToList(), out id);
                prototype.ID = id;
                prototype.SetCommunity(_community);
                _factByID.Add(prototype.ID, prototype);
                _factByMemento.Add(memento, prototype);

                // Short circuit if the object was already there.
                if (!saved)
                    return prototype;

                // Invalidate all of the queries affected by the new object.
                List<QueryInvalidator> invalidators;
                foreach (CorrespondenceFactType factType in _typeStrategy.GetAllTypesOfFact(prototype))
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
                                targetFactIds = _storageStrategy.QueryForIds(invalidator.TargetFacts, prototype.ID).ToList();

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

            foreach (Action invalidateAction in invalidateActions)
                invalidateAction();

            return prototype;
        }

        public T FindFact<T>(T prototype) where T : CorrespondenceFact
        {
            lock (this)
            {
                if (prototype.Community != null)
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

        public TimestampID LoadOutgoingTimestamp(string protocolName, string peerName)
        {
            return _storageStrategy.LoadOutgoingTimestamp(protocolName, peerName);
        }

        public void SaveOutgoingTimestamp(string protocolName, string peerName, TimestampID timestamp)
        {
            _storageStrategy.SaveOutgoingTimestamp(protocolName, peerName, timestamp);
        }

        public TimestampID LoadIncomingTimestamp(string protocolName, string peerName)
        {
            return _storageStrategy.LoadIncomingTimestamp(protocolName, peerName);
        }

        public void SaveIncomingTimestamp(string protocolName, string peerName, TimestampID timestamp)
        {
            _storageStrategy.SaveIncomingTimestamp(protocolName, peerName, timestamp);
        }

        public IEnumerable<MessageMemento> LoadRecentMessages(ref TimestampID timestamp)
        {
            return _storageStrategy.LoadRecentMessages(ref timestamp);
        }

        public IEnumerable<FactID> LoadRecentMessages(FactID pivotId, TimestampID timestamp)
        {
            return _storageStrategy.LoadRecentMessages(pivotId, timestamp);
        }

        public FactMemento LoadFact(FactID factId)
        {
            return CreateMementoFromFact(GetFactByID(factId));
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

        public FactID IDOfFact(CorrespondenceFact fact)
        {
            return fact.ID;
        }

        internal IEnumerable<CorrespondenceFact> ExecuteQuery(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            lock (this)
            {
                List<IdentifiedFactMemento> facts = _storageStrategy.QueryForFacts(queryDefinition, startingId, options).ToList();
                return facts.Select(m => GetFactByIdAndMemento(m.Id, m.Memento)).ToList();
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

        public CorrespondenceFact HydrateFact(FactMemento memento)
        {
            ICorrespondenceFactFactory factory;
            if (!_factoryByType.TryGetValue(memento.FactType, out factory))
                throw new CorrespondenceException(string.Format("Unknown type: {0}", memento.FactType));

            CorrespondenceFact fact = factory.CreateFact(memento);
            if (fact == null)
                throw new CorrespondenceException("Failed to create fact");
            return fact;
        }

        public FactID SaveFact(FactMemento translatedMemento)
        {
            CorrespondenceFact fact = HydrateFact(translatedMemento);
            fact = AddFact(fact);
            return fact.ID;
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
            }

            return memento;
        }

        private IEnumerable<FactID> FindPivots(CorrespondenceFact prototype)
        {
            return prototype.PredecessorRoles
                .Select(role => prototype.GetPredecessor(role))
                .Where(predecessor => AttributeTypeStrategy.IsPivot(predecessor.FactType))
                .SelectMany(predecessor => predecessor.InternalFactIds);
        }
    }
}
