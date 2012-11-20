using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Memory
{
    public class AsyncMemoryStorageStrategy : IStorageStrategy
    {
        private List<FactRecord> _factTable = new List<FactRecord>();
        private List<PeerRecord> _peerTable = new List<PeerRecord>();
        private List<MessageRecord> _messageTable = new List<MessageRecord>();
        private readonly IDictionary<int, TimestampID> _outgoingTimestampByPeer = new Dictionary<int, TimestampID>();
        private IDictionary<PeerPivotIdentifier, TimestampID> _incomingTimestampByPeerAndPivot = new Dictionary<PeerPivotIdentifier, TimestampID>();
        private List<ShareRecord> _shareTable = new List<ShareRecord>();
        private IDictionary<string, FactID> _namedFacts = new Dictionary<string, FactID>();

        private Guid _clientGuid = Guid.NewGuid();

        private Queue<Task<List<IdentifiedFactMemento>>> _futureTasks = new Queue<Task<List<IdentifiedFactMemento>>>();

        private AwaitableCriticalSection _lock = new AwaitableCriticalSection();

        public void Quiesce()
        {
            Task.WaitAll(_futureTasks.ToArray());
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

        public async Task<FactMemento> LoadAsync(FactID id)
        {
            using (await _lock.EnterAsync())
            {
                FactRecord factRecord = _factTable.FirstOrDefault(o => o.IdentifiedFactMemento.Id.Equals(id));
                if (factRecord != null)
                    return factRecord.IdentifiedFactMemento.Memento;
                else
                    throw new CorrespondenceException(string.Format("Fact with id {0} not found.", id));
            }
        }

        public async Task<SaveResult> SaveAsync(FactMemento memento, int peerId)
        {
            FactID id;
            bool wasSaved;

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
                    .Select(predecessor => new MessageRecord(
                        new MessageMemento(predecessor.ID, newFactID),
                        newFactID,
                        predecessor.Role)));

                // Store messages for each non-pivot. This fact belongs to all predecessors' pivots.
                List<FactID> nonPivots = memento.Predecessors
                    .Where(predecessor => !predecessor.IsPivot)
                    .Select(predecessor => predecessor.ID)
                    .ToList();
                List<MessageRecord> predecessorsPivots = _messageTable
                    .Where(message => nonPivots.Contains(message.Message.FactId))
                    .Distinct()
                    .ToList();
                _messageTable.AddRange(predecessorsPivots
                    .Select(predecessorPivot => new MessageRecord(
                        new MessageMemento(predecessorPivot.Message.PivotId, newFactID),
                        predecessorPivot.AncestorFact,
                        predecessorPivot.AncestorRole)));

                wasSaved = true;
            }
            else
            {
                id = fact.IdentifiedFactMemento.Id;
                wasSaved = false;
            }
            return new SaveResult { WasSaved = wasSaved, Id = id };
        }

        public async Task<FactID?> FindExistingFactAsync(FactMemento memento)
        {
            using (await _lock.EnterAsync())
            {
                // See if the fact already exists.
                FactRecord fact = _factTable.FirstOrDefault(o => o.IdentifiedFactMemento.Memento.Equals(memento));
                if (fact == null)
                {
                    return null;
                }
                else
                {
                    return fact.IdentifiedFactMemento.Id;
                }
            }
        }

        public Task<List<IdentifiedFactMemento>> QueryForFactsAsync(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            Task<List<IdentifiedFactMemento>> task = Task.Run(() =>
                new QueryExecutor(_factTable
                    .Select(f => f.IdentifiedFactMemento))
                .ExecuteQuery(queryDefinition, startingId, options)
                .Reverse()
                .ToList());
            _futureTasks.Enqueue(task);
            return task;
        }

        public async Task<IEnumerable<FactID>> QueryForIdsAsync(QueryDefinition queryDefinition, FactID startingId)
        {
            return new QueryExecutor(_factTable.Select(f => f.IdentifiedFactMemento)).ExecuteQuery(queryDefinition, startingId, null).Reverse().Select(im => im.Id);
        }

        public async Task<int> SavePeerAsync(string protocolName, string peerName)
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

        public async Task<TimestampID> LoadOutgoingTimestampAsync(int peerId)
        {
            TimestampID timestamp;
            if (_outgoingTimestampByPeer.TryGetValue(peerId, out timestamp))
                return timestamp;
            else
                return new TimestampID();
        }

        public async Task SaveOutgoingTimestampAsync(int peerId, TimestampID timestamp)
        {
            _outgoingTimestampByPeer[peerId] = timestamp;
        }

        public async Task<TimestampID> LoadIncomingTimestampAsync(int peerId, FactID pivotId)
        {
            TimestampID timestamp;
            if (_incomingTimestampByPeerAndPivot.TryGetValue(new PeerPivotIdentifier(peerId, pivotId), out timestamp))
                return timestamp;
            else
                return new TimestampID();
        }

        public async Task SaveIncomingTimestampAsync(int peerId, FactID pivotId, TimestampID timestamp)
        {
            _incomingTimestampByPeerAndPivot[new PeerPivotIdentifier(peerId, pivotId)] = timestamp;
        }

        public async Task<IEnumerable<MessageMemento>> LoadRecentMessagesForServerAsync(int peerId, TimestampID timestamp)
        {
            return _messageTable
                .Where(message =>
                    message.Message.FactId.key > timestamp.Key &&
                    !_factTable.Any(fact =>
                        fact.IdentifiedFactMemento.Id.Equals(message.Message.FactId) &&
                        fact.PeerId == peerId))
                .Select(message => message.Message)
                .Distinct()
                .ToList();
        }

        public async Task<FactID> GetFactIDFromShareAsync(int peerId, FactID remoteFactId)
        {
            var share = _shareTable.FirstOrDefault(s => s.PeerId == peerId && s.RemoteFactId.Equals(remoteFactId));
            if (share != null)
                return share.LocalFactId;

            throw new CorrespondenceException(String.Format("Share not found for peer {0} and remote fact {1}.", peerId, remoteFactId.key));
        }

        public async Task<FactID?> GetRemoteIdAsync(FactID localFactId, int peerId)
        {
            var share = _shareTable.FirstOrDefault(s => s.PeerId == peerId && s.LocalFactId.Equals(localFactId));
            if (share != null)
            {
                return share.RemoteFactId;
            }
            return null;
        }

        public async Task SaveShareAsync(int peerId, FactID remoteFactId, FactID localFactId)
        {
            _shareTable.Add(new ShareRecord
            {
                PeerId = peerId,
                RemoteFactId = remoteFactId,
                LocalFactId = localFactId
            });
        }
    }
}
