using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Memory
{
    public class MemoryStorageStrategy : IStorageStrategy
    {
        private List<IdentifiedMemento> _factTable = new List<IdentifiedMemento>();

        public IDisposable BeginUnitOfWork()
        {
            return new UnitOfWork();
        }

        public bool GetID(string factName, out FactID id)
        {
            throw new NotImplementedException();
        }

        public void SetID(string factName, FactID id)
        {
            throw new NotImplementedException();
        }

        public Memento Load(FactID id)
        {
            IdentifiedMemento fact = _factTable.FirstOrDefault(o => o.Id.Equals(id));
            if (fact != null)
                return fact.Memento;
            else
                throw new CorrespondenceException(string.Format("Fact with id {0} not found.", id));
        }

        public bool Save(Memento memento, out FactID id)
        {
            // See if the fact already exists.
            IdentifiedMemento fact = _factTable.FirstOrDefault(o => o.Memento.Equals(memento));
            if (fact == null)
            {
                // It doesn't, so create it.
                id = new FactID() { key = _factTable.Count };
                fact = new IdentifiedMemento(id, memento);
                _factTable.Add(fact);
                return true;
            }
            else
            {
                id = fact.Id;
                return false;
            }
        }

        public bool FindExistingFact(Memento memento, out FactID id)
        {
            // See if the fact already exists.
            IdentifiedMemento fact = _factTable.FirstOrDefault(o => o.Memento.Equals(memento));
            if (fact == null)
            {
                id = new FactID();
                return false;
            }
            else
            {
                id = fact.Id;
                return true;
            }
        }

        public IEnumerable<IdentifiedMemento> QueryForFacts(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            return new QueryExecutor(_factTable).ExecuteQuery(queryDefinition, startingId, options).Reverse();
        }

        public IEnumerable<FactID> QueryForIds(QueryDefinition queryDefinition, FactID startingId)
        {
            return QueryForFacts(queryDefinition, startingId, null).Select(im => im.Id);
        }


        public int SavePeer(string protocolName, string peerName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IdentifiedMemento> LoadAllFacts()
        {
            throw new NotImplementedException();
        }

        public FactID GetFactIDFromShare(int peerId, FactID remoteFactId)
        {
            throw new NotImplementedException();
        }

        public void SaveShare(int peerId, FactID remoteFactId, FactID localFactId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NamedFactMemento> LoadAllNamedFacts()
        {
            throw new NotImplementedException();
        }
    }
}
