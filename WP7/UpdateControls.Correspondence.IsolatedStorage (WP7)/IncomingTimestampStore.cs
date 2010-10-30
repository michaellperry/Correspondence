using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System;

namespace UpdateControls.Correspondence.IsolatedStorage
{
    public class IncomingTimestampStore
    {
        private const string IncomingTimestampTableFileName = "IncomingTimestampTable.bin";
        private IDictionary<PeerPivotIdentifier, TimestampID> _incomingTimestampByPeerAndPivot = new Dictionary<PeerPivotIdentifier, TimestampID>();

        private IncomingTimestampStore()
        {
        }

        public TimestampID LoadIncomingTimestamp(string protocolName, string peerName, FactID pivotId)
        {
            TimestampID timestamp;
            if (_incomingTimestampByPeerAndPivot.TryGetValue(new PeerPivotIdentifier(new PeerIdentifier(protocolName, peerName), pivotId), out timestamp))
                return timestamp;
            else
                return new TimestampID();
        }

        public void SaveIncomingTimestamp(string protocolName, string peerName, FactID pivotId, TimestampID timestamp, IsolatedStorageFile store)
        {
            _incomingTimestampByPeerAndPivot[new PeerPivotIdentifier(new PeerIdentifier(protocolName, peerName), pivotId)] = timestamp;

            using (BinaryWriter writer = new BinaryWriter(
                store.OpenFile(IncomingTimestampTableFileName,
                    FileMode.Create,
                    FileAccess.Write)))
            {
                IEnumerable<KeyValuePair<PeerPivotIdentifier, TimestampID>> timestamps = _incomingTimestampByPeerAndPivot;
                foreach (var entry in timestamps)
                {
                    writer.Write(entry.Key.PeerIdentifier.ProtocolName);
                    writer.Write(entry.Key.PeerIdentifier.PeerName);
                    writer.Write(entry.Key.PivotId.key);
                    writer.Write(entry.Value.DatabaseId);
                    writer.Write(entry.Value.Key);
                }
            }
        }

        public static IncomingTimestampStore Load(IsolatedStorageFile store)
        {
            IncomingTimestampStore result = new IncomingTimestampStore();

            if (store.FileExists(IncomingTimestampTableFileName))
            {
                using (BinaryReader timestampReader = new BinaryReader(
                    store.OpenFile(IncomingTimestampTableFileName,
                        FileMode.Open,
                        FileAccess.Read)))
                {
                    long length = timestampReader.BaseStream.Length;
                    while (timestampReader.BaseStream.Position < length)
                    {
                        result.ReadTimestampFromStorage(timestampReader);
                    }
                }
            }
            return result;
        }

        private void ReadTimestampFromStorage(BinaryReader timestampReader)
        {
            string protocolName = timestampReader.ReadString();
            string peerName = timestampReader.ReadString();
            long pivotKey = timestampReader.ReadInt64();
            long databaseId = timestampReader.ReadInt64();
            long timestamp = timestampReader.ReadInt64();

            _incomingTimestampByPeerAndPivot.Add(
                new PeerPivotIdentifier(
                    new PeerIdentifier(protocolName, peerName),
                    new FactID { key = pivotKey }),
                new TimestampID(databaseId, timestamp));
        }

        public static void DeleteAll(IsolatedStorageFile store)
        {
            if (store.FileExists(IncomingTimestampTableFileName))
                store.DeleteFile(IncomingTimestampTableFileName);
        }
    }
}
