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
        private List<IdentifiedFactMemento> _factTable = new List<IdentifiedFactMemento>();
        private List<MessageMemento> _messageTable = new List<MessageMemento>();
        private IDictionary<PeerIdentifier, TimestampID> _outgoingTimestampByPeer = new Dictionary<PeerIdentifier,TimestampID>();
        private IDictionary<PeerPivotIdentifier, TimestampID> _incomingTimestampByPeerAndPivot = new Dictionary<PeerPivotIdentifier, TimestampID>();
        private IDictionary<string, FactID> _namedFacts = new Dictionary<string, FactID>();

        public IDisposable BeginDuration()
        {
            return new Duration();
        }

        public bool GetID(string factName, out FactID id)
        {
            return _namedFacts.TryGetValue(factName, out id);
        }

        public void SetID(string factName, FactID id)
        {
            _namedFacts[factName] = id;
        }

        public FactMemento Load(FactID id)
        {
            IdentifiedFactMemento fact = _factTable.FirstOrDefault(o => o.Id.Equals(id));
            if (fact != null)
                return fact.Memento;
            else
                throw new CorrespondenceException(string.Format("Fact with id {0} not found.", id));
        }

        public bool Save(FactMemento memento, out FactID id)
        {
            // See if the fact already exists.
            IdentifiedFactMemento fact = _factTable.FirstOrDefault(o => o.Memento.Equals(memento));
            if (fact == null)
            {
                // It doesn't, so create it.
                FactID newFactID = new FactID() { key = _factTable.Count + 1 };
                id = newFactID;
                fact = new IdentifiedFactMemento(id, memento);
                _factTable.Add(fact);

                // Store a message for each pivot.
                _messageTable.AddRange(memento.Predecessors
                    .Where(predecessor => predecessor.Role.IsPivot)
                    .Select(predecessor => new MessageMemento(predecessor.ID, newFactID)));

                // Store messages for each non-pivot. This fact belongs to all predecessors' pivots.
                List<FactID> nonPivots = memento.Predecessors
                    .Where(predecessor => !predecessor.Role.IsPivot)
                    .Select(predecessor => predecessor.ID)
                    .ToList();
                List<FactID> predecessorsPivots = _messageTable
                    .Where(message => nonPivots.Contains(message.FactId))
                    .Select(message => message.PivotId)
                    .Distinct()
                    .ToList();
                _messageTable.AddRange(predecessorsPivots
                    .Select(predecessorPivot => new MessageMemento(predecessorPivot, newFactID)));

                return true;
            }
            else
            {
                id = fact.Id;
                return false;
            }
        }

        public bool FindExistingFact(FactMemento memento, out FactID id)
        {
            // See if the fact already exists.
            IdentifiedFactMemento fact = _factTable.FirstOrDefault(o => o.Memento.Equals(memento));
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

        public IEnumerable<IdentifiedFactMemento> QueryForFacts(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
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

        public TimestampID LoadOutgoingTimestamp(string protocolName, string peerName)
        {
            TimestampID timestamp;
            if (_outgoingTimestampByPeer.TryGetValue(new PeerIdentifier(protocolName, peerName), out timestamp))
                return timestamp;
            else
                return new TimestampID();
        }

        public void SaveOutgoingTimestamp(string protocolName, string peerName, TimestampID timestamp)
        {
            _outgoingTimestampByPeer[new PeerIdentifier(protocolName, peerName)] = timestamp;
        }

        public TimestampID LoadIncomingTimestamp(string protocolName, string peerName, FactID pivotId)
        {
            TimestampID timestamp;
            if (_incomingTimestampByPeerAndPivot.TryGetValue(new PeerPivotIdentifier(new PeerIdentifier(protocolName, peerName), pivotId), out timestamp))
                return timestamp;
            else
                return new TimestampID();
        }

        public void SaveIncomingTimestamp(string protocolName, string peerName, FactID pivotId, TimestampID timestamp)
        {
            _incomingTimestampByPeerAndPivot[new PeerPivotIdentifier(new PeerIdentifier(protocolName, peerName), pivotId)] = timestamp;
        }

        public IEnumerable<MessageMemento> LoadRecentMessages(TimestampID timestamp)
        {
            return _messageTable
                .Where(message => message.FactId.key > timestamp.Key)
                .ToList();
        }

        public IEnumerable<FactID> LoadRecentMessages(FactID pivotId, TimestampID timestamp)
        {
            return _messageTable
                .Where(message => message.PivotId.Equals(pivotId) && message.FactId.key > timestamp.Key)
                .Select(message => message.FactId);
        }

        public IEnumerable<IdentifiedFactMemento> LoadAllFacts()
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
