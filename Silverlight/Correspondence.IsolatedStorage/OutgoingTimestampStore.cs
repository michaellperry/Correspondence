using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using Correspondence.Mementos;

namespace Correspondence.IsolatedStorage
{
    public class OutgoingTimestampStore
    {
        private const string OutgoingTimestampTableFileName = "OutgoingTimestampTable.bin";
        private IDictionary<int, TimestampID> _outgoingTimestampByPeer = new Dictionary<int, TimestampID>();

        private OutgoingTimestampStore()
        {
        }

        public TimestampID LoadOutgoingTimestamp(int peerId)
        {
            TimestampID timestamp;
            if (_outgoingTimestampByPeer.TryGetValue(peerId, out timestamp))
                return timestamp;
            else
                return new TimestampID();
        }

        public void SaveOutgoingTimestamp(int peerId, TimestampID timestamp, IsolatedStorageFile store)
        {
            _outgoingTimestampByPeer[peerId] = timestamp;

            using (BinaryWriter writer = new BinaryWriter(
                store.OpenFile(OutgoingTimestampTableFileName,
                    FileMode.Create,
                    FileAccess.Write)))
            {
                var timestamps = _outgoingTimestampByPeer;
                foreach (var entry in timestamps)
                {
                    writer.Write(entry.Key);
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
            int peerId = timestampReader.ReadInt32();
            long databaseId = timestampReader.ReadInt64();
            long timestamp = timestampReader.ReadInt64();

            _outgoingTimestampByPeer.Add(
                peerId,
                new TimestampID(databaseId, timestamp));
        }

        public static void DeleteAll(IsolatedStorageFile store)
        {
            if (store.FileExists(OutgoingTimestampTableFileName))
                store.DeleteFile(OutgoingTimestampTableFileName);
        }
    }
}
