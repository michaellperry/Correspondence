using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;
using System.IO.IsolatedStorage;
using System.IO;

namespace UpdateControls.Correspondence.IsolatedStorage
{
    public class OutgoingTimestampStore
    {
        private const string OutgoingTimestampTableFileName = "OutgoingTimestampTable.bin";
        private IDictionary<PeerIdentifier, TimestampID> _outgoingTimestampByPeer = new Dictionary<PeerIdentifier, TimestampID>();

        private OutgoingTimestampStore()
        {
        }

        public TimestampID LoadOutgoingTimestamp(string protocolName, string peerName)
        {
            TimestampID timestamp;
            if (_outgoingTimestampByPeer.TryGetValue(new PeerIdentifier(protocolName, peerName), out timestamp))
                return timestamp;
            else
                return new TimestampID();
        }

        public void SaveOutgoingTimestamp(string protocolName, string peerName, TimestampID timestamp, IsolatedStorageFile store)
        {
            _outgoingTimestampByPeer[new PeerIdentifier(protocolName, peerName)] = timestamp;

            using (BinaryWriter writer = new BinaryWriter(
                store.OpenFile(OutgoingTimestampTableFileName,
                    FileMode.Create,
                    FileAccess.Write)))
            {
                var timestamps = _outgoingTimestampByPeer;
                foreach (var entry in timestamps)
                {
                    writer.Write(entry.Key.ProtocolName);
                    writer.Write(entry.Key.PeerName);
                    writer.Write(entry.Value.DatabaseId);
                    writer.Write(entry.Value.Key);
                }
            }
        }

        public static OutgoingTimestampStore Load(IsolatedStorageFile store)
        {
            OutgoingTimestampStore result = new OutgoingTimestampStore();

            if (store.FileExists(OutgoingTimestampTableFileName))
            {
                using (BinaryReader timestampReader = new BinaryReader(
                    store.OpenFile(OutgoingTimestampTableFileName,
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
            long databaseId = timestampReader.ReadInt64();
            long timestamp = timestampReader.ReadInt64();

            _outgoingTimestampByPeer.Add(
                new PeerIdentifier(protocolName, peerName),
                new TimestampID(databaseId, timestamp));
        }

        public static void DeleteAll(IsolatedStorageFile store)
        {
            if (store.FileExists(OutgoingTimestampTableFileName))
                store.DeleteFile(OutgoingTimestampTableFileName);
        }
    }
}
