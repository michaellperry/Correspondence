using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.IsolatedStorage
{
    public class FactTypeStore
    {
        private const string FactTypeTableFileName = "FactTypeTable.bin";
        private IDictionary<int, CorrespondenceFactType> _factTypeById = new Dictionary<int, CorrespondenceFactType>();

        private FactTypeStore()
        {
        }

        public CorrespondenceFactType GetFactType(int factTypeId)
        {
            CorrespondenceFactType factType;
            if (!_factTypeById.TryGetValue(factTypeId, out factType))
                throw new CorrespondenceException(String.Format("Fact type {0} is in tree, but is not recognized.", factTypeId));
            return factType;
        }

        public int GetFactTypeId(CorrespondenceFactType factType, IsolatedStorageFile store)
        {
            int factTypeId = _factTypeById
                .Where(pair => pair.Value.Equals(factType))
                .Select(pair => pair.Key)
                .FirstOrDefault();
            if (factTypeId == 0)
            {
                factTypeId = _factTypeById.Count + 1;
                _factTypeById.Add(factTypeId, factType);

                using (BinaryWriter writer = new BinaryWriter(
                    store.OpenFile(FactTypeTableFileName, FileMode.Append)))
                {
                    writer.Write(factTypeId);
                    writer.Write(factType.TypeName);
                    writer.Write(factType.Version);
                }
            }
            return factTypeId;
        }

        public static FactTypeStore Load(IsolatedStorageFile store)
        {
            FactTypeStore result = new FactTypeStore();

            if (store.FileExists(FactTypeTableFileName))
            {
                using (BinaryReader factTypeReader = new BinaryReader(store.OpenFile(
                    FactTypeTableFileName,
                    FileMode.Open,
                    FileAccess.Read)))
                {
                    result.ReadAllFactTypesFromStorage(factTypeReader);
                }
            }

            return result;
        }

        private void ReadAllFactTypesFromStorage(BinaryReader factTypeReader)
        {
            long length = factTypeReader.BaseStream.Length;
            while (factTypeReader.BaseStream.Position < length)
            {
                ReadFactTypeFromStorage(factTypeReader);
            }
        }

        private void ReadFactTypeFromStorage(BinaryReader factTypeReader)
        {
            int factTypeId = factTypeReader.ReadInt32();
            string typeName = factTypeReader.ReadString();
            int version = factTypeReader.ReadInt32();

            _factTypeById.Add(factTypeId, new CorrespondenceFactType(typeName, version));
        }
    }
}
