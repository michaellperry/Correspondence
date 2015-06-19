using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Correspondence.Mementos;

namespace Correspondence.FileStream
{
    internal class IncomingTimestampRecord
    {
        public PeerPivotIdentifier Id { get; set; }
        public TimestampID Timestamp { get; set; }

        public static IncomingTimestampRecord Read(BinaryReader reader)
        {
            int peerId = reader.ReadInt32();
            long pivotKey = reader.ReadInt64();
            long databaseId = reader.ReadInt64();
            long timestamp = reader.ReadInt64();

            return new IncomingTimestampRecord
            {
                Id = new PeerPivotIdentifier(
                    peerId,
                    new FactID { key = pivotKey }),
                Timestamp = new TimestampID(databaseId, timestamp)
            };
        }

        public static void Write(BinaryWriter writer, IncomingTimestampRecord record)
        {
            writer.Write(record.Id.PeerId);
            writer.Write(record.Id.PivotId.key);
            writer.Write(record.Timestamp.DatabaseId);
            writer.Write(record.Timestamp.Key);
        }
    }
}
