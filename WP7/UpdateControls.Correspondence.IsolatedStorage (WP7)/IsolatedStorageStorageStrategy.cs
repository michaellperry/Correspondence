using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.IsolatedStorage
{
    public class IsolatedStorageStorageStrategy : IStorageStrategy
    {
        private const string FactTableFileName = "FactTable.bin";
        private const string MessageTableFileName = "MessageTable.bin";

        private List<IdentifiedFactMemento> _factTable = new List<IdentifiedFactMemento>();
        private List<MessageMemento> _messageTable = new List<MessageMemento>();
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
                if (store.FileExists(FactTableFileName))
                {
                    using (BinaryReader factReader = new BinaryReader(
                        store.OpenFile(FactTableFileName,
                            FileMode.Open,
                            FileAccess.Read)))
                    {
                        ReadAllFactsFromStorage(result, factReader);
                    }
                }
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

                WriteFactToStorage(fact);

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
            return new QueryExecutor(_factTable).ExecuteQuery(queryDefinition, startingId, options)
                .OrderByDescending(identifiedFact => identifiedFact.Id.key);
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

        private static void ReadAllFactsFromStorage(IsolatedStorageStorageStrategy result, BinaryReader factReader)
        {
            long length = factReader.BaseStream.Length;
            while (factReader.BaseStream.Position < length)
            {
                ReadFactFromStorage(result, factReader);
            }
        }

        private static void ReadFactFromStorage(IsolatedStorageStorageStrategy result, BinaryReader factReader)
        {
            long factId;
            string typeName;
            int version;
            short dataSize;
            byte[] data;
            short predecessorCount;

            factId = factReader.ReadInt64();
            typeName = factReader.ReadString();
            version = factReader.ReadInt32();
            dataSize = factReader.ReadInt16();
            data = dataSize > 0 ? factReader.ReadBytes(dataSize) : new byte[0];
            predecessorCount = factReader.ReadInt16();

            FactMemento factMemento = new FactMemento(new CorrespondenceFactType(typeName, version));
            factMemento.Data = data;
            for (short i = 0; i < predecessorCount; i++)
            {
                string declaringTypeName;
                int declaringTypeVersion;
                string roleName;
                string targetTypeName;
                int targetTypeVersion;
                bool isPivot;
                long predecessorFactId;

                declaringTypeName = factReader.ReadString();
                declaringTypeVersion = factReader.ReadInt32();
                roleName = factReader.ReadString();
                targetTypeName = factReader.ReadString();
                targetTypeVersion = factReader.ReadInt32();
                isPivot = factReader.ReadBoolean();
                predecessorFactId = factReader.ReadInt64();

                factMemento.AddPredecessor(
                    new RoleMemento(
                        new CorrespondenceFactType(declaringTypeName, declaringTypeVersion),
                        roleName,
                        new CorrespondenceFactType(targetTypeName, targetTypeVersion),
                        isPivot),
                    new FactID() { key = predecessorFactId }
                );
            }
            result._factTable.Add(new IdentifiedFactMemento(new FactID { key = factId }, factMemento));
        }

        private static void WriteFactToStorage(IdentifiedFactMemento fact)
        {
            long factId = fact.Id.key;
            string typeName = fact.Memento.FactType.TypeName;
            int version = fact.Memento.FactType.Version;
            short dataSize = (short)fact.Memento.Data.Length;
            byte[] data = fact.Memento.Data;
            short predecessorCount = (short)fact.Memento.Predecessors.Count();

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (BinaryWriter factWriter = new BinaryWriter(
                        store.OpenFile(FactTableFileName,
                            FileMode.Append,
                            FileAccess.Write)))
                {
                    factWriter.Write(factId);
                    factWriter.Write(typeName);
                    factWriter.Write(version);
                    factWriter.Write(dataSize);
                    if (dataSize > 0)
                        factWriter.Write(data);
                    factWriter.Write(predecessorCount);

                    foreach (PredecessorMemento predecessor in fact.Memento.Predecessors)
                    {
                        string declaringTypeName = predecessor.Role.DeclaringType.TypeName;
                        int declaringTypeVersion = predecessor.Role.DeclaringType.Version;
                        string roleName = predecessor.Role.RoleName;
                        string targetTypeName = predecessor.Role.TargetType.TypeName;
                        int targetTypeVersion = predecessor.Role.TargetType.Version;
                        bool isPivot = predecessor.Role.IsPivot;
                        long predecessorFactId = predecessor.ID.key;

                        factWriter.Write(declaringTypeName);
                        factWriter.Write(declaringTypeVersion);
                        factWriter.Write(roleName);
                        factWriter.Write(targetTypeName);
                        factWriter.Write(targetTypeVersion);
                        factWriter.Write(isPivot);
                        factWriter.Write(predecessorFactId);
                    }
                }
            }
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
    }
}
