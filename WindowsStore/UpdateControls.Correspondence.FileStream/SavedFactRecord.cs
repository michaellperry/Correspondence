using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateControls.Correspondence.Mementos;
using System.IO;

namespace UpdateControls.Correspondence.FileStream
{
    internal class SavedFactRecord
    {
        public string Name { get; set; }
        public FactID Id { get; set; }

        public static SavedFactRecord Read(BinaryReader reader)
        {
            string name = reader.ReadString();
            long factId = reader.ReadInt64();

            return new SavedFactRecord
            {
                Name = name,
                Id = new FactID { key = factId }
            };
        }

        public static void Write(BinaryWriter writer, SavedFactRecord record)
        {
            writer.Write(record.Name);
            writer.Write(record.Id.key);
        }
    }
}
