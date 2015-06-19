using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using Correspondence.Mementos;

namespace Correspondence.IsolatedStorage
{
    public class MessageStore
    {
        private const string MessageTableFileName = "MessageTable.bin";
        private List<MessageRecord> _messageTable = new List<MessageRecord>();

        private MessageStore()
        {
        }

        public IEnumerable<MessageMemento> LoadRecentMessagesForServer(int peerId, TimestampID timestamp)
        {
            return _messageTable
                .Where(message =>
                    message.MessageMemento.FactId.key > timestamp.Key &&
                    message.SourcePeerId != peerId)
                .Select(message => message.MessageMemento)
                .ToList();
        }

        public IEnumerable<FactID> LoadRecentMessagesForClient(FactID pivotId, TimestampID timestamp)
        {
            return _messageTable
                .Where(message =>
                    message.MessageMemento.PivotId.Equals(pivotId) &&
                    message.MessageMemento.FactId.key > timestamp.Key)
                .Select(message => message.MessageMemento.FactId);
        }

        public List<FactID> GetPivotsOfFacts(List<FactID> factIds)
        {
            return _messageTable
                .Where(message => factIds.Contains(message.MessageMemento.FactId))
                .Select(message => message.MessageMemento.PivotId)
                .Distinct()
                .ToList();
        }

        public void AddMessages(IsolatedStorageFile store, List<MessageMemento> messages, int peerId)
        {
            _messageTable.AddRange(messages.Select(message => new MessageRecord
            {
                MessageMemento = message,
                SourcePeerId = peerId
            }));

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
                    factWriter.Write(peerId);
                }
            }
        }

        public static MessageStore Load(IsolatedStorageFile store)
        {
            MessageStore result = new MessageStore();

            if (store.FileExists(MessageTableFileName))
            {
                using (BinaryReader messageReader = new BinaryReader(
                    store.OpenFile(MessageTableFileName,
                        FileMode.Open,
                        FileAccess.Read)))
                {
                    long length = messageReader.BaseStream.Length;
                    while (messageReader.BaseStream.Position < length)
                    {
                        result.ReadMessageFromStorage(messageReader);
                    }
                }
            }

            return result;
        }

        private void ReadMessageFromStorage(BinaryReader messageReader)
        {
            long pivotId = messageReader.ReadInt64();
            long factId = messageReader.ReadInt64();
            int sourcePeerId = messageReader.ReadInt32();

            MessageMemento messageMemento = new MessageMemento(
                new FactID() { key = pivotId },
                new FactID() { key = factId });
            _messageTable.Add(new MessageRecord
            {
                MessageMemento = messageMemento,
                SourcePeerId = sourcePeerId
            });
        }

        public static void DeleteAll(IsolatedStorageFile store)
        {
            if (store.FileExists(MessageTableFileName))
            {
                store.DeleteFile(MessageTableFileName);
            }
        }
    }
}
