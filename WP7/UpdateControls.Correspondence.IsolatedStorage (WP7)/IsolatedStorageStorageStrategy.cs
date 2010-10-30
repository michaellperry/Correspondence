using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Data;

namespace UpdateControls.Correspondence.IsolatedStorage
{
    public class IsolatedStorageStorageStrategy : IStorageStrategy
    {
        private const string FactTreeFileName = "FactTable.bin";
        private const string IndexFileName = "Index.bin";
        private const string MessageTableFileName = "MessageTable.bin";

        private List<MessageMemento> _messageTable = new List<MessageMemento>();
        private IDictionary<int, CorrespondenceFactType> _factTypeById = new Dictionary<int, CorrespondenceFactType>();
        private IDictionary<int, RoleMemento> _roleById = new Dictionary<int, RoleMemento>();
        private IDictionary<PeerIdentifier, TimestampID> _outgoingTimestampByPeer = new Dictionary<PeerIdentifier, TimestampID>();
        private IDictionary<PeerPivotIdentifier, TimestampID> _incomingTimestampByPeerAndPivot = new Dictionary<PeerPivotIdentifier, TimestampID>();
        private IDictionary<string, FactID> _namedFacts = new Dictionary<string, FactID>();

        private IsolatedStorageStorageStrategy()
        {
        }

        public static IsolatedStorageStorageStrategy Load()
        {
            var result = new IsolatedStorageStorageStrategy();

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(MessageTableFileName))
                {
                    using (BinaryReader messageReader = new BinaryReader(
                        store.OpenFile(MessageTableFileName,
                            FileMode.Open,
                            FileAccess.Read)))
                    {
                        ReadAllMessagesFromStorage(result, messageReader);
                    }
                }
            }

            return result;
        }

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
                        int factTypeId = _factTypeById
                            .Where(pair => pair.Value.Equals(memento.FactType))
                            .Select(pair => pair.Key)
                            .FirstOrDefault();
                        if (factTypeId == 0)
                        {
                            factTypeId = _factTypeById.Count + 1;
                            _factTypeById.Add(factTypeId, memento.FactType);
                        }

                        HistoricalTreeFact historicalTreeFact = new HistoricalTreeFact(factTypeId, memento.Data);
                        foreach (PredecessorMemento predecessor in memento.Predecessors)
                        {
                            RoleMemento role = predecessor.Role;
                            int roleId = GetRoleId(role);
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
                        List<FactID> predecessorsPivots = _messageTable
                            .Where(message => nonPivots.Contains(message.FactId))
                            .Select(message => message.PivotId)
                            .Distinct()
                            .ToList();
                        IEnumerable<MessageMemento> indirectMessages = predecessorsPivots
                            .Select(predecessorPivot => new MessageMemento(predecessorPivot, newFactID));

                        List<MessageMemento> allMessages = directMessages
                            .Union(indirectMessages)
                            .ToList();
                        _messageTable.AddRange(allMessages);
                        WriteMessagesToStorage(allMessages);

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
                    return new QueryExecutor(factTree, GetRoleId)
                        .ExecuteQuery(queryDefinition, startingId.key, null)
                        .OrderByDescending(key => key)
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
                    return new QueryExecutor(factTree, GetRoleId)
                        .ExecuteQuery(queryDefinition, startingId.key, null)
                        .OrderByDescending(key => key)
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
            CorrespondenceFactType factType;
            if (!_factTypeById.TryGetValue(factNode.FactTypeId, out factType))
                throw new CorrespondenceException(String.Format("Fact type {0} is in tree, but is not recognized.", factNode.FactTypeId));

            FactMemento factMemento = new FactMemento(factType) { Data = factNode.Data };
            foreach (HistoricalTreePredecessor predecessorNode in factNode.Predecessors)
            {
                RoleMemento roleMemento;
                if (!_roleById.TryGetValue(predecessorNode.RoleId, out roleMemento))
                    throw new CorrespondenceException(String.Format("Role {0} is in tree, but is not recognized.", predecessorNode.RoleId));

                factMemento.AddPredecessor(roleMemento, new FactID { key = predecessorNode.PredecessorFactId });
            }
            return factMemento;
        }

        private static void ReadAllMessagesFromStorage(IsolatedStorageStorageStrategy result, BinaryReader messageReader)
        {
            long length = messageReader.BaseStream.Length;
            while (messageReader.BaseStream.Position < length)
            {
                ReadMessageFromStorage(result, messageReader);
            }
        }

        private static void ReadMessageFromStorage(IsolatedStorageStorageStrategy result, BinaryReader messageReader)
        {
            long pivotId;
            long factId;

            pivotId = messageReader.ReadInt64();
            factId = messageReader.ReadInt64();

            MessageMemento messageMemento = new MessageMemento(
                new FactID() { key = pivotId },
                new FactID() { key = factId });
            result._messageTable.Add(messageMemento);
        }

        private static void WriteMessagesToStorage(IEnumerable<MessageMemento> messages)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (BinaryWriter factWriter = new BinaryWriter(
                        store.OpenFile(MessageTableFileName,
                            FileMode.Append,
                            FileAccess.Write)))
                {
                    foreach (MessageMemento message in messages)
                    {
                        long pivotId = message.PivotId.key;
                        long factId = message.FactId.key;

                        factWriter.Write(pivotId);
                        factWriter.Write(factId);
                    }
                }
            }
        }

        private int GetRoleId(RoleMemento role)
        {
            int roleId = _roleById
                .Where(pair => pair.Value == role)
                .Select(pair => pair.Key)
                .FirstOrDefault();
            if (roleId == 0)
            {
                roleId = _roleById.Count + 1;
                _roleById.Add(roleId, role);
            }
            return roleId;
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
                if (store.FileExists(MessageTableFileName))
                {
                    store.DeleteFile(MessageTableFileName);
                }
            }
        }
    }
}
