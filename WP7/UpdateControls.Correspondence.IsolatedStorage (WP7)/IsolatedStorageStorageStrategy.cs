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
        private const string FactTreeFileName = "FactTree.bin";
        private const string IndexFileName = "Index.bin";

        private MessageStore _messageStore;
        private FactTypeStore _factTypeStore;
        private RoleStore _roleStore;
        private OutgoingTimestampStore _outgoingTimestampStore;
        private IncomingTimestampStore _incomingTimestampStore;

        private IsolatedStorageStorageStrategy()
        {
        }

        public static IsolatedStorageStorageStrategy Load()
        {
            var result = new IsolatedStorageStorageStrategy();

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                result._messageStore = MessageStore.Load(store);
                result._factTypeStore = FactTypeStore.Load(store);
                result._roleStore = RoleStore.Load(store);
                result._incomingTimestampStore = IncomingTimestampStore.Load(store);
                result._outgoingTimestampStore = OutgoingTimestampStore.Load(store);
            }

            return result;
        }

        public IDisposable BeginDuration()
        {
            return new Duration();
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
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (HistoricalTree factTree = OpenFactTree(store))
                {
                    return LoadFactFromTree(factTree, id.key);
                }
            }
        }

        public bool Save(FactMemento memento, out FactID id)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (RedBlackTree index = OpenIndex(store))
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
                    using (HistoricalTree factTree = OpenFactTree(store))
                    {
                        int factTypeId = _factTypeStore.GetFactTypeId(memento.FactType, store);
                        HistoricalTreeFact historicalTreeFact = new HistoricalTreeFact(factTypeId, memento.Data);
                        foreach (PredecessorMemento predecessor in memento.Predecessors)
                        {
                            RoleMemento role = predecessor.Role;
                            int roleId = _roleStore.GetRoleId(role, store);
                            historicalTreeFact.AddPredecessor(roleId, predecessor.ID.key);
                        }
                        long newFactIDKey = factTree.Save(historicalTreeFact);
                        index.AddFact(memento.GetHashCode(), newFactIDKey);
                        FactID newFactID = new FactID() { key = newFactIDKey };
                        id = newFactID;

                        // Store a message for each pivot.
                        IEnumerable<MessageMemento> directMessages = memento.Predecessors
                            .Where(predecessor => predecessor.Role.IsPivot)
                            .Select(predecessor => new MessageMemento(predecessor.ID, newFactID));

                        // Store messages for each non-pivot. This fact belongs to all predecessors' pivots.
                        List<FactID> nonPivots = memento.Predecessors
                            .Where(predecessor => !predecessor.Role.IsPivot)
                            .Select(predecessor => predecessor.ID)
                            .ToList();
                        List<FactID> predecessorsPivots = _messageStore.GetPivotsOfFacts(nonPivots);
                        IEnumerable<MessageMemento> indirectMessages = predecessorsPivots
                            .Select(predecessorPivot => new MessageMemento(predecessorPivot, newFactID));

                        List<MessageMemento> allMessages = directMessages
                            .Union(indirectMessages)
                            .ToList();
                        _messageStore.AddMessages(store, allMessages);
                        return true;
                    }
                }
            }
        }

        public bool FindExistingFact(FactMemento memento, out FactID id)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (RedBlackTree index = OpenIndex(store))
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
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (HistoricalTree factTree = OpenFactTree(store))
                {
                    return new QueryExecutor(factTree, role => _roleStore.GetRoleId(role, store))
                        .ExecuteQuery(queryDefinition, startingId.key, options)
                        .Select(key => new IdentifiedFactMemento(new FactID { key = key }, LoadFactFromTree(factTree, key)))
                        .ToList();
                }
            }
        }

        public IEnumerable<FactID> QueryForIds(QueryDefinition queryDefinition, FactID startingId)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (HistoricalTree factTree = OpenFactTree(store))
                {
                    return new QueryExecutor(factTree, role => _roleStore.GetRoleId(role, store))
                        .ExecuteQuery(queryDefinition, startingId.key, null)
                        .Select(key => new FactID { key = key })
                        .ToList();
                }
            }
        }


        public int SavePeer(string protocolName, string peerName)
        {
            throw new NotImplementedException();
        }

        public TimestampID LoadOutgoingTimestamp(string protocolName, string peerName)
        {
            return _outgoingTimestampStore.LoadOutgoingTimestamp(protocolName, peerName);
        }

        public void SaveOutgoingTimestamp(string protocolName, string peerName, TimestampID timestamp)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                _outgoingTimestampStore.SaveOutgoingTimestamp(protocolName, peerName, timestamp, store);
            }
        }

        public TimestampID LoadIncomingTimestamp(string protocolName, string peerName, FactID pivotId)
        {
            return _incomingTimestampStore.LoadIncomingTimestamp(protocolName, peerName, pivotId);
        }

        public void SaveIncomingTimestamp(string protocolName, string peerName, FactID pivotId, TimestampID timestamp)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                _incomingTimestampStore.SaveIncomingTimestamp(protocolName, peerName, pivotId, timestamp, store);
            }
        }

        public IEnumerable<MessageMemento> LoadRecentMessages(TimestampID timestamp)
        {
            return _messageStore.LoadRecentMessages(timestamp);
        }

        public IEnumerable<FactID> LoadRecentMessages(FactID pivotId, TimestampID timestamp)
        {
            return _messageStore.LoadRecentMessages(pivotId, timestamp);
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
                factMemento.AddPredecessor(role, new FactID { key = predecessorNode.PredecessorFactId });
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
                IncomingTimestampStore.DeleteAll(store);
                OutgoingTimestampStore.DeleteAll(store);
            }
        }
    }
}
