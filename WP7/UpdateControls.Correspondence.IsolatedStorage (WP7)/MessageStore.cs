using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.IsolatedStorage
{
    public class MessageStore
    {
        private const string MessageTableFileName = "MessageTable.bin";
        private List<MessageMemento> _messageTable = new List<MessageMemento>();

        private MessageStore()
        {
        }

        public IEnumerable<MessageMemento> LoadRecentMessagesForServer(TimestampID timestamp)
        {
            return _messageTable
                .Where(message => message.FactId.key > timestamp.Key)
                .ToList();
        }

        public IEnumerable<FactID> LoadRecentMessagesForClient(FactID pivotId, TimestampID timestamp)
        {
            return _messageTable
                .Where(message => message.PivotId.Equals(pivotId) && message.FactId.key > timestamp.Key)
                .Select(message => message.FactId);
        }

        public List<FactID> GetPivotsOfFacts(List<FactID> factIds)
        {
            return _messageTable
                .Where(message => factIds.Contains(message.FactId))
                .Select(message => message.PivotId)
                .Distinct()
                .ToList();
        }

        public void AddMessages(IsolatedStorageFile store, List<MessageMemento> messages)
        {
            _messageTable.AddRange(messages);

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
            long pivotId;
            long factId;

            pivotId = messageReader.ReadInt64();
            factId = messageReader.ReadInt64();

            MessageMemento messageMemento = new MessageMemento(
                new FactID() { key = pivotId },
                new FactID() { key = factId });
            _messageTable.Add(messageMemento);
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
