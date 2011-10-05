using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using UpdateControls.Correspondence.Data;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.IsolatedStorage
{
    public class IsolatedStorageStorageStrategy : IStorageStrategy
    {
        private const string ClientGuidFileName = "ClientGuid.bin";
        private const string FactTreeFileName = "FactTree.bin";
        private const string IndexFileName = "Index.bin";

        private IsolatedStorageFile _store;

        private Guid _clientGuid;
        private MessageStore _messageStore;
        private FactTypeStore _factTypeStore;
        private RoleStore _roleStore;
        private PeerStore _peerStore;
        private OutgoingTimestampStore _outgoingTimestampStore;
        private IncomingTimestampStore _incomingTimestampStore;
        private SavedFactStore _savedFactStore;

        private IsolatedStorageStorageStrategy(IsolatedStorageFile store)
        {
            _store = store;
        }

        public static IsolatedStorageStorageStrategy Load()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            var result = new IsolatedStorageStorageStrategy(store);

            result._messageStore = MessageStore.Load(store);
            result._factTypeStore = FactTypeStore.Load(store);
            result._roleStore = RoleStore.Load(store);
            result._peerStore = PeerStore.Load(store);
            result._incomingTimestampStore = IncomingTimestampStore.Load(store);
            result._outgoingTimestampStore = OutgoingTimestampStore.Load(store);
            result._savedFactStore = SavedFactStore.Load(store);
            if (store.FileExists(ClientGuidFileName))
            {
                using (BinaryReader input = new BinaryReader(store.OpenFile(ClientGuidFileName, FileMode.Open)))
                {
                    result._clientGuid = new Guid(input.ReadBytes(16));
                }
            }
            else
            {
                result._clientGuid = Guid.NewGuid();
                using (BinaryWriter output = new BinaryWriter(store.OpenFile(ClientGuidFileName, FileMode.Create)))
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
            return _savedFactStore.GetFactId(factName, out id);
        }

        public void SetID(string factName, FactID id)
        {
            _savedFactStore.SaveFactId(factName, id, _store);
        }

        public FactMemento Load(FactID id)
        {
            lock (this)
            {
                using (HistoricalTree factTree = OpenFactTree(_store))
                {
                    return LoadFactFromTree(factTree, id.key);
                }
            }
        }

        public bool Save(FactMemento memento, int peerId, out FactID id)
        {
            lock (this)
            {
                using (RedBlackTree index = OpenIndex(_store))
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
                    using (HistoricalTree factTree = OpenFactTree(_store))
                    {
                        int factTypeId = _factTypeStore.GetFactTypeId(memento.FactType, _store);
                        HistoricalTreeFact historicalTreeFact = new HistoricalTreeFact(factTypeId, memento.Data);
                        foreach (PredecessorMemento predecessor in memento.Predecessors)
                        {
                            RoleMemento role = predecessor.Role;
                            int roleId = _roleStore.GetRoleId(role, _store);
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
                        _messageStore.AddMessages(_store, allMessages, peerId);
                        return true;
                    }
                }
            }
        }

        public bool FindExistingFact(FactMemento memento, out FactID id)
        {
            lock (this)
            {
                using (RedBlackTree index = OpenIndex(_store))
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
        }

        public IEnumerable<IdentifiedFactMemento> QueryForFacts(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            lock (this)
            {
                using (HistoricalTree factTree = OpenFactTree(_store))
                {
                    return new QueryExecutor(factTree, role => _roleStore.GetRoleId(role, _store))
                        .ExecuteQuery(queryDefinition, startingId.key, options)
                        .Select(key => new IdentifiedFactMemento(new FactID { key = key }, LoadFactFromTree(factTree, key)))
                        .ToList();
                }
            }
        }

        public IEnumerable<FactID> QueryForIds(QueryDefinition queryDefinition, FactID startingId)
        {
            lock (this)
            {
                using (HistoricalTree factTree = OpenFactTree(_store))
                {
                    return new QueryExecutor(factTree, role => _roleStore.GetRoleId(role, _store))
                        .ExecuteQuery(queryDefinition, startingId.key, null)
                        .Select(key => new FactID { key = key })
                        .ToList();
                }
            }
        }

        public int SavePeer(string protocolName, string peerName)
        {
            lock (this)
            {
                return _peerStore.SavePeer(protocolName, peerName, _store);
            }
        }

        public TimestampID LoadOutgoingTimestamp(int peerId)
        {
            lock (this)
            {
                return _outgoingTimestampStore.LoadOutgoingTimestamp(peerId);
            }
        }

        public void SaveOutgoingTimestamp(int peerId, TimestampID timestamp)
        {
            lock (this)
            {
                _outgoingTimestampStore.SaveOutgoingTimestamp(peerId, timestamp, _store);
            }
        }

        public TimestampID LoadIncomingTimestamp(int peerId, FactID pivotId)
        {
            lock (this)
            {
                return _incomingTimestampStore.LoadIncomingTimestamp(peerId, pivotId);
            }
        }

        public void SaveIncomingTimestamp(int peerId, FactID pivotId, TimestampID timestamp)
        {
            lock (this)
            {
                _incomingTimestampStore.SaveIncomingTimestamp(peerId, pivotId, timestamp, _store);
            }
        }

        public IEnumerable<MessageMemento> LoadRecentMessagesForServer(int peerId, TimestampID timestamp)
        {
            lock (this)
            {
                return _messageStore.LoadRecentMessagesForServer(peerId, timestamp);
            }
        }

        public IEnumerable<FactID> LoadRecentMessagesForClient(FactID pivotId, TimestampID timestamp)
        {
            lock (this)
            {
                return _messageStore.LoadRecentMessagesForClient(pivotId, timestamp);
            }
        }

        public void Unpublish(FactID factId, RoleMemento role)
        {
            throw new NotImplementedException();
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

        private static HistoricalTree OpenFactTree(IsolatedStorageFile store)
        {
            Stream factTreeStream = store.OpenFile(
                FactTreeFileName,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite);
            HistoricalTree _factTree = new HistoricalTree(factTreeStream);
            return _factTree;
        }

        private static RedBlackTree OpenIndex(IsolatedStorageFile store)
        {
            Stream indexStream = store.OpenFile(
                IndexFileName,
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

        public static void DeleteAll()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(FactTreeFileName))
                {
                    store.DeleteFile(FactTreeFileName);
                }
                if (store.FileExists(IndexFileName))
                {
                    store.DeleteFile(IndexFileName);
                }
                MessageStore.DeleteAll(store);
                FactTypeStore.DeleteAll(store);
                RoleStore.DeleteAll(store);
                PeerStore.DeleteAll(store);
                IncomingTimestampStore.DeleteAll(store);
                OutgoingTimestampStore.DeleteAll(store);
            }
        }
    }
}
