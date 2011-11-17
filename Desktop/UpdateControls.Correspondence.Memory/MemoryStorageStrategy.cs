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
        private List<FactRecord> _factTable = new List<FactRecord>();
        private List<PeerRecord> _peerTable = new List<PeerRecord>();
        private List<MessageMemento> _messageTable = new List<MessageMemento>();
        private readonly IDictionary<int, TimestampID> _outgoingTimestampByPeer = new Dictionary<int, TimestampID>();
        private IDictionary<PeerPivotIdentifier, TimestampID> _incomingTimestampByPeerAndPivot = new Dictionary<PeerPivotIdentifier, TimestampID>();
        private List<ShareRecord> _shareTable = new List<ShareRecord>();
        private IDictionary<string, FactID> _namedFacts = new Dictionary<string, FactID>();

        private Guid _clientGuid = Guid.NewGuid();

        public IDisposable BeginDuration()
        {
            return new Duration();
        }

		public Guid ClientGuid
		{
            get { return _clientGuid; }
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
            FactRecord factRecord = _factTable.FirstOrDefault(o => o.IdentifiedFactMemento.Id.Equals(id));
            if (factRecord != null)
                return factRecord.IdentifiedFactMemento.Memento;
            else
                throw new CorrespondenceException(string.Format("Fact with id {0} not found.", id));
        }

        public bool Save(FactMemento memento, int peerId, out FactID id)
        {
            // See if the fact already exists.
            FactRecord fact = _factTable.FirstOrDefault(o => o.IdentifiedFactMemento.Memento.Equals(memento));
            if (fact == null)
            {
                // It doesn't, so create it.
                FactID newFactID = new FactID() { key = _factTable.Count + 1 };
                id = newFactID;
                fact = new FactRecord()
                {
                    IdentifiedFactMemento = new IdentifiedFactMemento(id, memento),
                    PeerId = peerId
                };

                _factTable.Add(fact);

                // Store a message for each pivot.
                _messageTable.AddRange(memento.Predecessors
                    .Where(predecessor => predecessor.IsPivot)
                    .Select(predecessor => new MessageMemento(predecessor.ID, newFactID)));

                // Store messages for each non-pivot. This fact belongs to all predecessors' pivots.
                List<FactID> nonPivots = memento.Predecessors
                    .Where(predecessor => !predecessor.IsPivot)
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
                id = fact.IdentifiedFactMemento.Id;
                return false;
            }
        }

        public bool FindExistingFact(FactMemento memento, out FactID id)
        {
            // See if the fact already exists.
            FactRecord fact = _factTable.FirstOrDefault(o => o.IdentifiedFactMemento.Memento.Equals(memento));
            if (fact == null)
            {
                id = new FactID();
                return false;
            }
            else
            {
                id = fact.IdentifiedFactMemento.Id;
                return true;
            }
        }

        public IEnumerable<IdentifiedFactMemento> QueryForFacts(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            return new QueryExecutor(_factTable.Select(f => f.IdentifiedFactMemento))
                .ExecuteQuery(queryDefinition, startingId, options)
                .Reverse();
        }

        public IEnumerable<FactID> QueryForIds(QueryDefinition queryDefinition, FactID startingId)
        {
            return QueryForFacts(queryDefinition, startingId, null).Select(im => im.Id);
        }

        public int SavePeer(string protocolName, string peerName)
        {
            PeerRecord peerRecord = _peerTable.FirstOrDefault(peer =>
                peer.ProtocolName == protocolName &&
                peer.PeerName == peerName);
            if (peerRecord == null)
            {
                peerRecord = new PeerRecord
                {
                    PeerId = _peerTable.Count + 1,
                    ProtocolName = protocolName,
                    PeerName = peerName
                };
            }
            return peerRecord.PeerId;
        }

        public TimestampID LoadOutgoingTimestamp(int peerId)
        {
            TimestampID timestamp;
            if (_outgoingTimestampByPeer.TryGetValue(peerId, out timestamp))
                return timestamp;
            else
                return new TimestampID();
        }

        public void SaveOutgoingTimestamp(int peerId, TimestampID timestamp)
        {
            _outgoingTimestampByPeer[peerId] = timestamp;
        }

        public TimestampID LoadIncomingTimestamp(int peerId, FactID pivotId)
        {
            TimestampID timestamp;
            if (_incomingTimestampByPeerAndPivot.TryGetValue(new PeerPivotIdentifier(peerId, pivotId), out timestamp))
                return timestamp;
            else
                return new TimestampID();
        }

        public void SaveIncomingTimestamp(int peerId, FactID pivotId, TimestampID timestamp)
        {
            _incomingTimestampByPeerAndPivot[new PeerPivotIdentifier(peerId, pivotId)] = timestamp;
        }

        public IEnumerable<MessageMemento> LoadRecentMessagesForServer(int peerId, TimestampID timestamp)
        {
            return _messageTable
                .Where(message =>
                    message.FactId.key > timestamp.Key &&
                    !_factTable.Any(fact =>
                        fact.IdentifiedFactMemento.Id.Equals(message.FactId) &&
                        fact.PeerId == peerId))
                .ToList();
        }

        public IEnumerable<FactID> LoadRecentMessagesForClient(FactID pivotId, TimestampID timestamp)
        {
            return _messageTable
                .Where(message => message.PivotId.Equals(pivotId) && message.FactId.key > timestamp.Key)
                .Select(message => message.FactId);
        }

        public void Unpublish(FactID factId, RoleMemento role)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IdentifiedFactMemento> LoadAllFacts()
        {
            return _factTable.Select(record => record.IdentifiedFactMemento);
        }

        public FactID GetFactIDFromShare(int peerId, FactID remoteFactId)
        {
            var share = _shareTable.FirstOrDefault(s => s.PeerId == peerId && s.RemoteFactId.Equals(remoteFactId));
            if (share != null)
                return share.LocalFactId;

            throw new CorrespondenceException(String.Format("Share not found for peer {0} and remote fact {1}.", peerId, remoteFactId.key));
        }

        public bool GetRemoteId(FactID localFactId, int peerId, out FactID remoteFactId)
        {
            var share = _shareTable.FirstOrDefault(s => s.PeerId == peerId && s.LocalFactId.Equals(localFactId));
            if (share != null)
            {
                remoteFactId = share.RemoteFactId;
                return true;
            }
            remoteFactId = new FactID();
            return false;
        }

        public void SaveShare(int peerId, FactID remoteFactId, FactID localFactId)
        {
            _shareTable.Add(new ShareRecord
            {
                PeerId = peerId,
                RemoteFactId = remoteFactId,
                LocalFactId = localFactId
            });
        }

        public IEnumerable<NamedFactMemento> LoadAllNamedFacts()
        {
            throw new NotImplementedException();
        }
    }
}
