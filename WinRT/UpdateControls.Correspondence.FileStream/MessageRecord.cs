using System.IO;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.FileStream
{
    internal class MessageRecord
    {
        public MessageMemento MessageMemento { get; set; }
        public int SourcePeerId { get; set; }

        public static MessageRecord Read(BinaryReader reader)
        {
            long pivotId = reader.ReadInt64();
            long factId = reader.ReadInt64();
            int sourcePeerId = reader.ReadInt32();

            MessageMemento messageMemento = new MessageMemento(
                new FactID() { key = pivotId },
                new FactID() { key = factId });
            return new MessageRecord
            {
                MessageMemento = messageMemento,
                SourcePeerId = sourcePeerId
            };
        }

        public static void Write(BinaryWriter writer, MessageRecord record)
        {
            writer.Write(record.MessageMemento.PivotId.key);
            writer.Write(record.MessageMemento.FactId.key);
            writer.Write(record.SourcePeerId);
        }
    }
}
