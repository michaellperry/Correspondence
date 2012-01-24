using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UpdateControls.Correspondence.Data;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.FileStream
{
    public class FileStreamStorageStrategy : IStorageStrategy
    {
        private const string ClientGuidFileName = "ClientGuid.bin";
        private const string FactTreeFileName = "FactTree.bin";
        private const string IndexFileName = "Index.bin";

        private string _filePath;

        private Guid _clientGuid;
        private MessageStore _messageStore;
        private FactTypeStore _factTypeStore;
        private RoleStore _roleStore;
        private PeerStore _peerStore;
        private OutgoingTimestampStore _outgoingTimestampStore;
        private IncomingTimestampStore _incomingTimestampStore;

        private FileStreamStorageStrategy(string filePath)
        {
            _filePath = filePath;
        }

        public static FileStreamStorageStrategy Load(string filePath)
        {
            var result = new FileStreamStorageStrategy(filePath);

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            result._messageStore = MessageStore.Load(filePath);
            result._factTypeStore = FactTypeStore.Load(filePath);
            result._roleStore = RoleStore.Load(filePath);
            result._peerStore = PeerStore.Load(filePath);
            result._incomingTimestampStore = IncomingTimestampStore.Load(filePath);
            result._outgoingTimestampStore = OutgoingTimestampStore.Load(filePath);
            string clientGuidFilePath = Path.Combine(filePath, ClientGuidFileName);
            if (File.Exists(clientGuidFilePath))
            {
                using (BinaryReader input = new BinaryReader(File.Open(clientGuidFilePath, FileMode.Open)))
                {
                    result._clientGuid = new Guid(input.ReadBytes(16));
                }
            }
            else
            {
                result._clientGuid = Guid.NewGuid();
                using (BinaryWriter output = new BinaryWriter(File.Open(clientGuidFilePath, FileMode.Create)))
                {
                    output.Write(result._clientGuid.ToByteArray());
                }
            }

            return result;
        }

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
            throw new NotImplementedException();
        }

        public void SetID(string factName, FactID id)
        {
            throw new NotImplementedException();
        }

        public FactMemento Load(FactID id)
        {
            using (HistoricalTree factTree = OpenFactTree())
            {
                return LoadFactFromTree(factTree, id.key);
            }
        }

        public bool Save(FactMemento memento, int peerId, out FactID id)
        {
            using (RedBlackTree index = OpenIndex())
            {
                // See if the fact already exists.
                foreach (long candidate in index.FindFacts(memento.GetHashCode()))
                {
                    FactID candidateId = new FactID() { key = candidate };
                    if (Load(candidateId).Equals(memento))
                    {
                        id = candidateId;
                        return false;
                    }
                }

                // It doesn't, so create it.
                using (HistoricalTree factTree = OpenFactTree())
                {
                    int factTypeId = _factTypeStore.GetFactTypeId(memento.FactType);
                    HistoricalTreeFact historicalTreeFact = new HistoricalTreeFact(factTypeId, memento.Data);
                    foreach (PredecessorMemento predecessor in memento.Predecessors)
                    {
                        RoleMemento role = predecessor.Role;
                        int roleId = _roleStore.GetRoleId(role);
                        historicalTreeFact.AddPredecessor(roleId, predecessor.ID.key);
                    }
                    long newFactIDKey = factTree.Save(historicalTreeFact);
                    index.AddFact(memento.GetHashCode(), newFactIDKey);
                    FactID newFactID = new FactID() { key = newFactIDKey };
                    id = newFactID;

                    // Store a message for each pivot.
                    IEnumerable<MessageMemento> directMessages = memento.Predecessors
                        .Where(predecessor => predecessor.IsPivot)
                        .Select(predecessor => new MessageMemento(predecessor.ID, newFactID));

                    // Store messages for each non-pivot. This fact belongs to all predecessors' pivots.
                    List<FactID> nonPivots = memento.Predecessors
                        .Where(predecessor => !predecessor.IsPivot)
                        .Select(predecessor => predecessor.ID)
                        .ToList();
                    List<FactID> predecessorsPivots = _messageStore.GetPivotsOfFacts(nonPivots);
                    IEnumerable<MessageMemento> indirectMessages = predecessorsPivots
                        .Select(predecessorPivot => new MessageMemento(predecessorPivot, newFactID));

                    List<MessageMemento> allMessages = directMessages
                        .Union(indirectMessages)
                        .ToList();
                    _messageStore.AddMessages(allMessages, peerId);
                    return true;
                }
            }
        }

        public bool FindExistingFact(FactMemento memento, out FactID id)
        {
            using (RedBlackTree index = OpenIndex())
            {
                // See if the fact already exists.
                foreach (long candidate in index.FindFacts(memento.GetHashCode()))
                {
                    FactID candidateId = new FactID() { key = candidate };
                    if (Load(candidateId).Equals(memento))
                    {
                        id = candidateId;
                        return false;
                    }
                }
                id = new FactID();
                return false;
            }
        }

        public IEnumerable<IdentifiedFactMemento> QueryForFacts(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            using (HistoricalTree factTree = OpenFactTree())
            {
                return new QueryExecutor(factTree, role => _roleStore.GetRoleId(role))
                    .ExecuteQuery(queryDefinition, startingId.key, options)
                    .Select(key => new IdentifiedFactMemento(new FactID { key = key }, LoadFactFromTree(factTree, key)))
                    .ToList();
            }
        }

        public IEnumerable<FactID> QueryForIds(QueryDefinition queryDefinition, FactID startingId)
        {
            using (HistoricalTree factTree = OpenFactTree())
            {
                return new QueryExecutor(factTree, role => _roleStore.GetRoleId(role))
                    .ExecuteQuery(queryDefinition, startingId.key, null)
                    .Select(key => new FactID { key = key })
                    .ToList();
            }
        }

        public int SavePeer(string protocolName, string peerName)
        {
            return _peerStore.SavePeer(protocolName, peerName);
        }

        public TimestampID LoadOutgoingTimestamp(int peerId)
        {
            return _outgoingTimestampStore.LoadOutgoingTimestamp(peerId);
        }

        public void SaveOutgoingTimestamp(int peerId, TimestampID timestamp)
        {
            _outgoingTimestampStore.SaveOutgoingTimestamp(peerId, timestamp);
        }

        public TimestampID LoadIncomingTimestamp(int peerId, FactID pivotId)
        {
            return _incomingTimestampStore.LoadIncomingTimestamp(peerId, pivotId);
        }

        public void SaveIncomingTimestamp(int peerId, FactID pivotId, TimestampID timestamp)
        {
            _incomingTimestampStore.SaveIncomingTimestamp(peerId, pivotId, timestamp);
        }

        public IEnumerable<MessageMemento> LoadRecentMessagesForServer(int peerId, TimestampID timestamp)
        {
            return _messageStore.LoadRecentMessagesForServer(peerId, timestamp);
        }

        public IEnumerable<FactID> LoadRecentMessagesForClient(FactID pivotId, TimestampID timestamp)
        {
            return _messageStore.LoadRecentMessagesForClient(pivotId, timestamp);
        }

        public void Unpublish(FactID factId, RoleMemento role)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IdentifiedFactMemento> LoadAllFacts()
        {
            throw new NotImplementedException();
        }

        public IdentifiedFactMemento LoadNextFact(FactID? lastFactId)
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

        private HistoricalTree OpenFactTree()
        {
            string factTreeFileName = Path.Combine(_filePath, FactTreeFileName);
            Stream factTreeStream = File.Open(
                factTreeFileName,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite);
            HistoricalTree _factTree = new HistoricalTree(factTreeStream);
            return _factTree;
        }

        private RedBlackTree OpenIndex()
        {
            string indexFileName = Path.Combine(_filePath, IndexFileName);
            Stream indexStream = File.Open(
                indexFileName,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite);
            return new RedBlackTree(indexStream);
        }

        private FactMemento LoadFactFromTree(HistoricalTree factTree, long key)
        {
            HistoricalTreeFact factNode = factTree.Load(key);
            CorrespondenceFactType factType = _factTypeStore.GetFactType(factNode.FactTypeId);
            FactMemento factMemento = new FactMemento(factType) { Data = factNode.Data };
            foreach (HistoricalTreePredecessor predecessorNode in factNode.Predecessors)
            {
                RoleMemento role = _roleStore.GetRole(predecessorNode.RoleId);
                factMemento.AddPredecessor(role, new FactID { key = predecessorNode.PredecessorFactId }, role.IsPivot);
            }
            return factMemento;
        }

        public static void DeleteAll(string filePath)
        {
            string factTreeFileName = Path.Combine(filePath, FactTreeFileName);
            if (File.Exists(factTreeFileName))
            {
                File.Delete(factTreeFileName);
            }
            string indexFileName = Path.Combine(filePath, IndexFileName);
            if (File.Exists(indexFileName))
            {
                File.Delete(indexFileName);
            }
            MessageStore.DeleteAll(filePath);
            FactTypeStore.DeleteAll(filePath);
            RoleStore.DeleteAll(filePath);
            PeerStore.DeleteAll(filePath);
            IncomingTimestampStore.DeleteAll(filePath);
            OutgoingTimestampStore.DeleteAll(filePath);
        }


        public bool GetRemoteId(FactID localFactId, int peerId, out FactID remoteFactId)
        {
            throw new NotImplementedException();
        }
    }
}
