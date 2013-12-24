using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.IsolatedStorage
{
    public class SavedFactStore
    {
        private const string SavedFactFileName = "SavedFact.bin";
        private IDictionary<string, FactID> _factIdByName = new Dictionary<string, FactID>();

        private SavedFactStore()
        {
        }

        public bool GetFactId(string name, out FactID factId)
        {
            return _factIdByName.TryGetValue(name, out factId);
        }

        public void SaveFactId(string name, FactID factId, IsolatedStorageFile store)
        {
            _factIdByName[name] = factId;

            using (var writer = new BinaryWriter(store.OpenFile(SavedFactFileName, FileMode.Create, FileAccess.Write)))
            {
                foreach (var entry in _factIdByName)
                {
                    writer.Write(entry.Key);
                    writer.Write(entry.Value.key);
                }
            }
        }

        public static SavedFactStore Load(IsolatedStorageFile store)
        {
            SavedFactStore savedFactStore = new SavedFactStore();
            if (store.FileExists(SavedFactFileName))
            {
                using (BinaryReader savedFactReader = new BinaryReader(store.OpenFile(
                    SavedFactFileName,
                    FileMode.Open,
                    FileAccess.Read)))
                {
                    long length = savedFactReader.BaseStream.Length;
                    while (savedFactReader.BaseStream.Position < length)
                    {
                        savedFactStore.ReadSavedFactFromStorage(savedFactReader);
                    }
                }
            }
            return savedFactStore;
        }

        public void ReadSavedFactFromStorage(BinaryReader savedFactReader)
        {
            string name = savedFactReader.ReadString();
            long factId = savedFactReader.ReadInt64();

            _factIdByName.Add(name, new FactID { key = factId });
        }

        public static void DeleteAll(IsolatedStorageFile store)
        {
            if (store.FileExists(SavedFactFileName))
                store.DeleteFile(SavedFactFileName);
        }
    }
}
