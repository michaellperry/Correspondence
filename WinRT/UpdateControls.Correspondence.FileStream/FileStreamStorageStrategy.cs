using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UpdateControls.Correspondence.Data;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.FileStream
{
    public class FileStreamStorageStrategy : IStorageStrategy
    {
        private Table<PeerRecord> _peerTable = new Table<PeerRecord>(
            "PeerTable.bin", PeerRecord.Read, PeerRecord.Write);

        public Task<Guid> GetClientGuidAsync()
        {
            throw new NotImplementedException();
        }

        public Task<FactID?> GetIDAsync(string factName)
        {
            throw new NotImplementedException();
        }

        public Task SetIDAsync(string factName, FactID id)
        {
            throw new NotImplementedException();
        }

        public Task<FactMemento> LoadAsync(FactID id)
        {
            throw new NotImplementedException();
        }

        public Task<SaveResult> SaveAsync(FactMemento memento, int peerId)
        {
            throw new NotImplementedException();
        }

        public Task<FactID?> FindExistingFactAsync(FactMemento memento)
        {
            throw new NotImplementedException();
        }

        public Task<List<IdentifiedFactMemento>> QueryForFactsAsync(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FactID>> QueryForIdsAsync(QueryDefinition queryDefinition, FactID startingId)
        {
            throw new NotImplementedException();
        }

        public Task<TimestampID> LoadOutgoingTimestampAsync(int peerId)
        {
            throw new NotImplementedException();
        }

        public Task SaveOutgoingTimestampAsync(int peerId, TimestampID timestamp)
        {
            throw new NotImplementedException();
        }

        public Task<TimestampID> LoadIncomingTimestampAsync(int peerId, FactID pivotId)
        {
            throw new NotImplementedException();
        }

        public Task SaveIncomingTimestampAsync(int peerId, FactID pivotId, TimestampID timestamp)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MessageMemento>> LoadRecentMessagesForServerAsync(int peerId, TimestampID timestamp)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SavePeerAsync(string protocolName, string peerName)
        {
            await _peerTable.LoadAsync();
            PeerRecord peer = _peerTable.Records
                .FirstOrDefault(row =>
                    row.ProtocolName == protocolName &&
                    row.PeerName == peerName);
            if (peer == null)
            {
                peer = new PeerRecord
                {
                    ProtocolName = protocolName,
                    PeerName = peerName,
                    PeerId = _peerTable.Records.Count() + 1
                };
                await _peerTable.SaveAsync(peer);
            }

            return peer.PeerId;
        }

        public Task<FactID> GetFactIDFromShareAsync(int peerId, FactID remoteFactId)
        {
            throw new NotImplementedException();
        }

        public Task<FactID?> GetRemoteIdAsync(FactID localFactId, int peerId)
        {
            throw new NotImplementedException();
        }

        public Task SaveShareAsync(int peerId, FactID remoteFactId, FactID localFactId)
        {
            throw new NotImplementedException();
        }
    }
}
