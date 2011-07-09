using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.FileStream
{
    public class FactTypeStore
    {
        private const string FactTypeTableFileName = "FactTypeTable.bin";
        private string _fileName;
        private IDictionary<int, CorrespondenceFactType> _factTypeById = new Dictionary<int, CorrespondenceFactType>();

        public FactTypeStore(string fileName)
        {
            _fileName = fileName;
        }

        public CorrespondenceFactType GetFactType(int factTypeId)
        {
            CorrespondenceFactType factType;
            if (!_factTypeById.TryGetValue(factTypeId, out factType))
                throw new CorrespondenceException(String.Format("Fact type {0} is in tree, but is not recognized.", factTypeId));
            return factType;
        }

        public int GetFactTypeId(CorrespondenceFactType factType)
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
                    File.Open(_fileName, FileMode.Append)))
                {
                    writer.Write(factTypeId);
                    writer.Write(factType.TypeName);
                    writer.Write(factType.Version);
                }
            }
            return factTypeId;
        }

        public static FactTypeStore Load(string filePath)
        {
            string fileName = Path.Combine(filePath, FactTypeTableFileName);
            FactTypeStore result = new FactTypeStore(fileName);

            if (File.Exists(fileName))
            {
                using (BinaryReader factTypeReader = new BinaryReader(File.Open(
                    fileName,
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

        public static void DeleteAll(string filePath)
        {
            string fileName = Path.Combine(filePath, FactTypeTableFileName);
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }
}
