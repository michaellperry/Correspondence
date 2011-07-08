using System.Collections.Generic;
using System.IO;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.FileStream
{
    public class OutgoingTimestampStore
    {
        private const string OutgoingTimestampTableFileName = "OutgoingTimestampTable.bin";
        private string _fileName;
        private IDictionary<int, TimestampID> _outgoingTimestampByPeer = new Dictionary<int, TimestampID>();

        public OutgoingTimestampStore(string fileName)
        {
            _fileName = fileName;
        }

        public TimestampID LoadOutgoingTimestamp(int peerId)
        {
            TimestampID timestamp;
            if (_outgoingTimestampByPeer.TryGetValue(peerId, out timestamp))
                return timestamp;
            else
                return new TimestampID();
        }

        public void SaveOutgoingTimestamp(int peerId, TimestampID timestamp)
        {
            _outgoingTimestampByPeer[peerId] = timestamp;

            using (BinaryWriter writer = new BinaryWriter(
                File.Open(_fileName,
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

        public static OutgoingTimestampStore Load(string filePath)
        {
            string fileName = Path.Combine(filePath, OutgoingTimestampTableFileName);
            OutgoingTimestampStore result = new OutgoingTimestampStore(fileName);

            if (File.Exists(fileName))
            {
                using (BinaryReader timestampReader = new BinaryReader(
                    File.Open(fileName,
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

        public static void DeleteAll(string filePath)
        {
            string fileName = Path.Combine(filePath, OutgoingTimestampTableFileName);
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }
}
