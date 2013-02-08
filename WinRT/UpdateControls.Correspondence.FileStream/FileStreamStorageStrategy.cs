using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UpdateControls.Correspondence.Data;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;
using System.Threading.Tasks;
using Windows.Storage;

namespace UpdateControls.Correspondence.FileStream
{
    public class FileStreamStorageStrategy : IStorageStrategy
    {
        private Table<Guid> _clientGuidTable = new Table<Guid>(
            "ClientGuid.bin", ReadGuid, WriteGuid);
        private Table<PeerRecord> _peerTable = new Table<PeerRecord>(
            "PeerTable.bin", PeerRecord.Read, PeerRecord.Write);
        private Table<MessageRecord> _messageTable = new Table<MessageRecord>(
            "MessageTable.bin", MessageRecord.Read, MessageRecord.Write);
        private Table<SavedFactRecord> _savedFactTable = new Table<SavedFactRecord>(
            "SavedFact.bin", SavedFactRecord.Read, SavedFactRecord.Write);
        private Table<OutgoingTimestampRecord> _outgoingTimestampTable = new Table<OutgoingTimestampRecord>(
            "OutgoingTimestampTable.bin", OutgoingTimestampRecord.Read, OutgoingTimestampRecord.Write);
        private Table<IncomingTimestampRecord> _incomingTimestampTable = new Table<IncomingTimestampRecord>(
            "IncomingTimestampTable.bin", IncomingTimestampRecord.Read, IncomingTimestampRecord.Write);
        private Table<FactTypeRecord> _factTypeTable = new Table<FactTypeRecord>(
            "FactTypeTable.bin", FactTypeRecord.Read, FactTypeRecord.Write);
        private Table<RoleRecord> _roleTable = new Table<RoleRecord>(
            "RoleTable.bin", RoleRecord.Read, RoleRecord.Write);

        private StorageFolder _correspondenceFolder;
        private StorageFile _indexBin;
        private StorageFile _factTreeBin;

        private RedBlackTree.NodeCache _nodeCache = new RedBlackTree.NodeCache();

        public async Task<Guid> GetClientGuidAsync()
        {
            await _clientGuidTable.LoadAsync();
            if (!_clientGuidTable.Records.Any())
            {
                Guid guid = Guid.NewGuid();
                await _clientGuidTable.AppendAsync(guid);
            }
            return _clientGuidTable.Records.First();
        }

        public async Task<FactID?> GetIDAsync(string factName)
        {
            await _savedFactTable.LoadAsync();
            var record = _savedFactTable.Records
                .FirstOrDefault(r => r.Name == factName);
            if (record == null)
                return null;
            else
                return record.Id;
        }

        public async Task SetIDAsync(string factName, FactID id)
        {
            await _savedFactTable.LoadAsync();
            var record = _savedFactTable.Records
                .FirstOrDefault(r => r.Name == factName);
            await _savedFactTable.ReplaceAsync(
                record,
                new SavedFactRecord
                {
                    Name = factName,
                    Id = id
                });
        }

        public async Task<FactMemento> LoadAsync(FactID id)
        {
            using (HistoricalTree factTree = await OpenFactTreeAsync())
            {
                return await LoadFactFromTreeAsync(factTree, id.key);
            }
        }

        public async Task<SaveResult> SaveAsync(FactMemento memento, int peerId)
        {
            using (var index = await OpenIndexAsync())
            {
                List<long> candidateFactIds = await
                    index.FindFactsAsync(memento.GetHashCode());

                // See if the fact already exists.
                foreach (long candidate in candidateFactIds)
                {
                    FactID candidateId = new FactID() { key = candidate };
                    FactMemento candidateFact = await LoadAsync(candidateId);
                    if (candidateFact.Equals(memento))
                    {
                        return new SaveResult
                        {
                            Id = candidateId,
                            WasSaved = false
                        };
                    }
                }

                // It doesn't, so create it.
                int factTypeId = await GetFactTypeIdAsync(memento.FactType);
                HistoricalTreeFact historicalTreeFact = new HistoricalTreeFact(factTypeId, memento.Data);
                foreach (PredecessorMemento predecessor in memento.Predecessors)
                {
                    RoleMemento role = predecessor.Role;
                    int roleId = await GetRoleIdAsync(role);
                    historicalTreeFact.AddPredecessor(roleId, predecessor.ID.key);
                }
                FactID newFactID;
                using (var factTree = await OpenFactTreeAsync())
                {
                    long newFactIDKey = await factTree.SaveAsync(historicalTreeFact);
                    await index.AddFactAsync(memento.GetHashCode(), newFactIDKey);
                    newFactID = new FactID() { key = newFactIDKey };
                }

                // Store a message for each pivot.
                IEnumerable<MessageMemento> directMessages = memento.Predecessors
                    .Where(predecessor => predecessor.IsPivot)
                    .Select(predecessor => new MessageMemento(predecessor.ID, newFactID));

                // Store messages for each non-pivot. This fact belongs to all predecessors' pivots.
                List<FactID> nonPivots = memento.Predecessors
                    .Where(predecessor => !predecessor.IsPivot)
                    .Select(predecessor => predecessor.ID)
                    .ToList();
                List<FactID> predecessorsPivots = await GetPivotsOfFactsAsync(nonPivots);
                IEnumerable<MessageMemento> indirectMessages = predecessorsPivots
                    .Select(predecessorPivot => new MessageMemento(predecessorPivot, newFactID));

                List<MessageMemento> allMessages = directMessages
                    .Union(indirectMessages)
                    .ToList();
                await AddMessagesAsync(allMessages, peerId);

                return new SaveResult
                {
                    Id = newFactID,
                    WasSaved = true
                };
            }
        }

        public async Task<FactID?> FindExistingFactAsync(FactMemento memento)
        {
            using (var index = await OpenIndexAsync())
            {
                List<long> candidateFactIds = await
                    index.FindFactsAsync(memento.GetHashCode());

                // See if the fact already exists.
                foreach (long candidate in candidateFactIds)
                {
                    FactID candidateId = new FactID() { key = candidate };
                    FactMemento candidateFact = await LoadAsync(candidateId);
                    if (candidateFact.Equals(memento))
                    {
                        return candidateId;
                    }
                }

                return null;
            }
        }

        public async Task<List<IdentifiedFactMemento>> QueryForFactsAsync(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            using (HistoricalTree factTree = await OpenFactTreeAsync())
            {
                QueryExecutor queryExecutor = new QueryExecutor(factTree, GetRoleIdAsync);
                List<long> factIds = await queryExecutor
                    .ExecuteQuery(queryDefinition, startingId.key, options);
                List<IdentifiedFactMemento> facts = new List<IdentifiedFactMemento>();
                foreach (var factId in factIds)
                {
                    FactMemento fact = await LoadFactFromTreeAsync(factTree, factId);
                    facts.Add(new IdentifiedFactMemento(new FactID { key = factId }, fact));
                }
                return facts;
            }
        }

        public async Task<List<FactID>> QueryForIdsAsync(QueryDefinition queryDefinition, FactID startingId)
        {
            using (HistoricalTree factTree = await OpenFactTreeAsync())
            {
                QueryExecutor queryExecutor = new QueryExecutor(factTree, GetRoleIdAsync);
                List<long> factIds = await queryExecutor
                    .ExecuteQuery(queryDefinition, startingId.key, null);
                return factIds.Select(key => new FactID{ key = key }).ToList();
            }
        }

        public async Task<TimestampID> LoadOutgoingTimestampAsync(int peerId)
        {
            await _outgoingTimestampTable.LoadAsync();
            var record = _outgoingTimestampTable.Records
                .FirstOrDefault(r => r.PeerId == peerId);
            if (record == null)
                return new TimestampID();
            else
                return record.Timestamp;
        }

        public async Task SaveOutgoingTimestampAsync(int peerId, TimestampID timestamp)
        {
            await _outgoingTimestampTable.LoadAsync();
            var record = _outgoingTimestampTable.Records
                .FirstOrDefault(r => r.PeerId == peerId);
            await _outgoingTimestampTable.ReplaceAsync(
                record,
                new OutgoingTimestampRecord
                {
                    PeerId = peerId,
                    Timestamp = timestamp
                });
        }

        public async Task<TimestampID> LoadIncomingTimestampAsync(int peerId, FactID pivotId)
        {
            await _incomingTimestampTable.LoadAsync();
            var record = _incomingTimestampTable.Records
                .FirstOrDefault(r => r.Id.PeerId == peerId && r.Id.PivotId.key == pivotId.key);
            if (record == null)
                return new TimestampID();
            else
                return record.Timestamp;
        }

        public async Task SaveIncomingTimestampAsync(int peerId, FactID pivotId, TimestampID timestamp)
        {
            await _incomingTimestampTable.LoadAsync();
            var record = _incomingTimestampTable.Records
                .FirstOrDefault(r => r.Id.PeerId == peerId && r.Id.PivotId.key == pivotId.key);
            await _incomingTimestampTable.ReplaceAsync(
                record,
                new IncomingTimestampRecord
                {
                    Id = new PeerPivotIdentifier(peerId, pivotId),
                    Timestamp = timestamp
                });
        }

        public async Task<List<MessageMemento>> LoadRecentMessagesForServerAsync(int peerId, TimestampID timestamp)
        {
            await _messageTable.LoadAsync();
            return _messageTable.Records
                .Where(message =>
                    message.MessageMemento.FactId.key > timestamp.Key &&
                    message.SourcePeerId != peerId)
                .Select(message => message.MessageMemento)
                .ToList();
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
                await _peerTable.AppendAsync(peer);
            }

            return peer.PeerId;
        }

        public Task<FactID> GetFactIDFromShareAsync(int peerId, FactID remoteFactId)
        {
            throw new NotImplementedException();
        }

        public Task<FactID?> GetRemoteIdAsync(FactID localFactId, int peerId)
        {
            return Task.FromResult<FactID?>(null);
        }

        public Task SaveShareAsync(int peerId, FactID remoteFactId, FactID localFactId)
        {
            return Task.WhenAll();
        }

        private static Guid ReadGuid(BinaryReader reader)
        {
            return new Guid(reader.ReadBytes(16));
        }

        private static void WriteGuid(BinaryWriter writer, Guid guid)
        {
            writer.Write(guid.ToByteArray());
        }

        private async Task<int> GetFactTypeIdAsync(CorrespondenceFactType factType)
        {
            await _factTypeTable.LoadAsync();
            var record = _factTypeTable.Records
                .FirstOrDefault(r => Object.Equals(r.FactType, factType));
            if (record == null)
            {
                record = new FactTypeRecord
                {
                    Id = _factTypeTable.Records.Count() + 1,
                    FactType = factType
                };
                await _factTypeTable.AppendAsync(record);
            }

            return record.Id;
        }

        private async Task<CorrespondenceFactType> GetFactTypeAsync(int factTypeId)
        {
            await _factTypeTable.LoadAsync();
            var record = _factTypeTable.Records
                .FirstOrDefault(r => r.Id == factTypeId);
            if (record == null)
                throw new CorrespondenceException(String.Format("Fact tree contains type {0}, which is not defined.", factTypeId));
            
            return record.FactType;
        }

        private async Task<int> GetRoleIdAsync(RoleMemento role)
        {
            await _roleTable.LoadAsync();
            var record = _roleTable.Records
                .FirstOrDefault(r => Object.Equals(r.Role, role));
            if (record == null)
            {
                record = new RoleRecord
                {
                    Id = _roleTable.Records.Count() + 1,
                    Role = role
                };
                await _roleTable.AppendAsync(record);
            }

            return record.Id;
        }

        private async Task<RoleMemento> GetRoleAsync(int roleId)
        {
            await _roleTable.LoadAsync();
            var record = _roleTable.Records
                .FirstOrDefault(r => r.Id == roleId);
            if (record == null)
                throw new CorrespondenceException(String.Format("Fact tree contains role {0}, which is not defined.", roleId));

            return record.Role;
        }

        private async Task<HistoricalTree> OpenFactTreeAsync()
        {
            var tableFile = await GetFactTreeBinAsync();

            var randomAccessStream = await tableFile.OpenAsync(FileAccessMode.ReadWrite);
            return new HistoricalTree(randomAccessStream);
        }

        private async Task<RedBlackTree> OpenIndexAsync()
        {
            var tableFile = await GetIndexBinAsync();

            var randomAccessStream = await tableFile.OpenAsync(FileAccessMode.ReadWrite);
            return new RedBlackTree(randomAccessStream, _nodeCache);
        }

        private async Task<FactMemento> LoadFactFromTreeAsync(HistoricalTree factTree, long key)
        {
            HistoricalTreeFact factNode = await factTree.LoadAsync(key);
            CorrespondenceFactType factType = await GetFactTypeAsync(factNode.FactTypeId);
            FactMemento factMemento = new FactMemento(factType) { Data = factNode.Data };
            foreach (HistoricalTreePredecessor predecessorNode in factNode.Predecessors)
            {
                RoleMemento role = await GetRoleAsync(predecessorNode.RoleId);
                factMemento.AddPredecessor(
                    role,
                    new FactID { key = predecessorNode.PredecessorFactId },
                    role.IsPivot);
            }
            return factMemento;
        }

        private async Task<List<FactID>> GetPivotsOfFactsAsync(List<FactID> factIds)
        {
            await _messageTable.LoadAsync();
            return _messageTable.Records
                .Where(message => factIds.Contains(message.MessageMemento.FactId))
                .Select(message => message.MessageMemento.PivotId)
                .Distinct()
                .ToList();
        }

        private async Task AddMessagesAsync(List<MessageMemento> messages, int peerId)
        {
            await _messageTable.LoadAsync();
            foreach (var message in messages)
            {
                await _messageTable.AppendAsync(new MessageRecord
                {
                    MessageMemento = message,
                    SourcePeerId = peerId
                });
            }
        }

        private async Task<StorageFile> GetIndexBinAsync()
        {
            if (_indexBin == null)
                _indexBin = await GetFileAsync("Index.bin");
            return _indexBin;
        }

        private async Task<StorageFile> GetFactTreeBinAsync()
        {
            if (_factTreeBin == null)
                _factTreeBin = await GetFileAsync("FactTree.bin");
            return _factTreeBin;
        }

        private async Task<StorageFolder> GetCorrespondenceFolderAsync()
        {
            if (_correspondenceFolder == null)
            {
                _correspondenceFolder = await ApplicationData.Current.LocalFolder
                    .CreateFolderAsync("Correspondence", CreationCollisionOption.OpenIfExists);
            }
            return _correspondenceFolder;
        }

        private async Task<StorageFile> GetFileAsync(string fileName)
        {
            StorageFolder correspondenceFolder = await GetCorrespondenceFolderAsync();
            var tableFile = await correspondenceFolder
                .CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            return tableFile;
        }
    }
}
