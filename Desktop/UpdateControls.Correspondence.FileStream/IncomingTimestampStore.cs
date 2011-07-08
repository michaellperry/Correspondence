using System.Collections.Generic;
using System.IO;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.FileStream
{
    public class IncomingTimestampStore
    {
        private const string IncomingTimestampTableFileName = "IncomingTimestampTable.bin";
        private string _fileName;
        private IDictionary<PeerPivotIdentifier, TimestampID> _incomingTimestampByPeerAndPivot = new Dictionary<PeerPivotIdentifier, TimestampID>();

        public IncomingTimestampStore(string fileName)
        {
            _fileName = fileName;
        }

        public TimestampID LoadIncomingTimestamp(int peerId, FactID pivotId)
        {
            TimestampID timestamp;
            if (_incomingTimestampByPeerAndPivot.TryGetValue(new PeerPivotIdentifier(peerId, pivotId), out timestamp))
                return timestamp;
            else
                return new TimestampID();
        }

        public void SaveIncomingTimestamp(int peerId, FactID pivotId, TimestampID timestamp)
        {
            _incomingTimestampByPeerAndPivot[new PeerPivotIdentifier(peerId, pivotId)] = timestamp;

            using (BinaryWriter writer = new BinaryWriter(
                File.Open(_fileName,
                    FileMode.Create,
                    FileAccess.Write)))
            {
                IEnumerable<KeyValuePair<PeerPivotIdentifier, TimestampID>> timestamps = _incomingTimestampByPeerAndPivot;
                foreach (var entry in timestamps)
                {
                    writer.Write(entry.Key.PeerId);
                    writer.Write(entry.Key.PivotId.key);
                    writer.Write(entry.Value.DatabaseId);
                    writer.Write(entry.Value.Key);
                }
            }
        }

        public static IncomingTimestampStore Load(string filePath)
        {
            string fileName = Path.Combine(filePath, IncomingTimestampTableFileName);
            IncomingTimestampStore result = new IncomingTimestampStore(fileName);

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
            long pivotKey = timestampReader.ReadInt64();
            long databaseId = timestampReader.ReadInt64();
            long timestamp = timestampReader.ReadInt64();

            _incomingTimestampByPeerAndPivot.Add(
                new PeerPivotIdentifier(
                    peerId,
                    new FactID { key = pivotKey }),
                new TimestampID(databaseId, timestamp));
        }

        public static void DeleteAll(string filePath)
        {
            string fileName = Path.Combine(filePath, IncomingTimestampTableFileName);
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }
}
