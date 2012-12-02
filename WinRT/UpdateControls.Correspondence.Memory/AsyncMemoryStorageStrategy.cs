﻿using System;
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

        private static Task Done = Task.WhenAll();

        public void Quiesce()
        {
            Task.WaitAll(_futureTasks.ToArray());
        }

        public Task<Guid> GetClientGuidAsync()
        {
            return Task.FromResult(_clientGuid);
        }

        public Task<FactID?> GetIDAsync(string factName)
        {
            FactID id;
            if (_namedFacts.TryGetValue(factName, out id))
                return Task.FromResult<FactID?>(id);
            else
                return Task.FromResult<FactID?>(null);
        }

        public Task SetIDAsync(string factName, FactID id)
        {
            _namedFacts[factName] = id;
            return Done;
        }

        public async Task<FactMemento> LoadAsync(FactID id)
        {
            FactRecord factRecord = _factTable.FirstOrDefault(o => o.IdentifiedFactMemento.Id.Equals(id));
            if (factRecord != null)
                return factRecord.IdentifiedFactMemento.Memento;
            else
                throw new CorrespondenceException(string.Format("Fact with id {0} not found.", id));
        }

        public Task<SaveResult> SaveAsync(FactMemento memento, int peerId)
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
            return Task.FromResult(new SaveResult { WasSaved = wasSaved, Id = id });
        }

        public async Task<FactID?> FindExistingFactAsync(FactMemento memento)
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

        public Task<List<FactID>> QueryForIdsAsync(QueryDefinition queryDefinition, FactID startingId)
        {
            return Task.FromResult(new QueryExecutor(
                _factTable.Select(f => f.IdentifiedFactMemento))
                .ExecuteQuery(queryDefinition, startingId, null)
                .Reverse()
                .Select(im => im.Id)
                .ToList());
        }

        public Task<int> SavePeerAsync(string protocolName, string peerName)
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
            return Task.FromResult(peerRecord.PeerId);
        }

        public Task<TimestampID> LoadOutgoingTimestampAsync(int peerId)
        {
            TimestampID timestamp;
            if (_outgoingTimestampByPeer.TryGetValue(peerId, out timestamp))
                return Task.FromResult(timestamp);
            else
                return Task.FromResult(new TimestampID());
        }

        public Task SaveOutgoingTimestampAsync(int peerId, TimestampID timestamp)
        {
            _outgoingTimestampByPeer[peerId] = timestamp;
            return Done;
        }

        public Task<TimestampID> LoadIncomingTimestampAsync(int peerId, FactID pivotId)
        {
            TimestampID timestamp;
            if (_incomingTimestampByPeerAndPivot.TryGetValue(new PeerPivotIdentifier(peerId, pivotId), out timestamp))
                return Task.FromResult(timestamp);
            else
                return Task.FromResult(new TimestampID());
        }

        public Task SaveIncomingTimestampAsync(int peerId, FactID pivotId, TimestampID timestamp)
        {
            _incomingTimestampByPeerAndPivot[new PeerPivotIdentifier(peerId, pivotId)] = timestamp;
            return Done;
        }

        public Task<List<MessageMemento>> LoadRecentMessagesForServerAsync(int peerId, TimestampID timestamp)
        {
            return Task.FromResult(_messageTable
                .Where(message =>
                    message.Message.FactId.key > timestamp.Key &&
                    !_factTable.Any(fact =>
                        fact.IdentifiedFactMemento.Id.Equals(message.Message.FactId) &&
                        fact.PeerId == peerId))
                .Select(message => message.Message)
                .Distinct()
                .ToList());
        }

        public Task<FactID> GetFactIDFromShareAsync(int peerId, FactID remoteFactId)
        {
            var share = _shareTable.FirstOrDefault(s => s.PeerId == peerId && s.RemoteFactId.Equals(remoteFactId));
            if (share != null)
                return Task.FromResult(share.LocalFactId);

            throw new CorrespondenceException(String.Format("Share not found for peer {0} and remote fact {1}.", peerId, remoteFactId.key));
        }

        public Task<FactID?> GetRemoteIdAsync(FactID localFactId, int peerId)
        {
            var share = _shareTable.FirstOrDefault(s => s.PeerId == peerId && s.LocalFactId.Equals(localFactId));
            if (share != null)
            {
                return Task.FromResult<FactID?>(share.RemoteFactId);
            }
            return Task.FromResult<FactID?>(null);
        }

        public Task SaveShareAsync(int peerId, FactID remoteFactId, FactID localFactId)
        {
            _shareTable.Add(new ShareRecord
            {
                PeerId = peerId,
                RemoteFactId = remoteFactId,
                LocalFactId = localFactId
            });
            return Done;
        }
    }
}
