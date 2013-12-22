using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;
using System.IO;

namespace UpdateControls.Correspondence.FileStream
{
    internal class OutgoingTimestampRecord
    {
        public int PeerId { get; set; }
        public TimestampID Timestamp { get; set; }

        public static OutgoingTimestampRecord Read(BinaryReader reader)
        {
            int peerId = reader.ReadInt32();
            long databaseId = reader.ReadInt64();
            long timestamp = reader.ReadInt64();

            return new OutgoingTimestampRecord
            {
                PeerId = peerId,
                Timestamp = new TimestampID(databaseId, timestamp)
            };
        }

        public static void Write(BinaryWriter writer, OutgoingTimestampRecord record)
        {
            writer.Write(record.PeerId);
            writer.Write(record.Timestamp.DatabaseId);
            writer.Write(record.Timestamp.Key);
        }
    }
}
