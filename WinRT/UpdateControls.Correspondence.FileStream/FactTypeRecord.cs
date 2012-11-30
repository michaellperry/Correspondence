using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;
using System.IO;

namespace UpdateControls.Correspondence.FileStream
{
    public class FactTypeRecord
    {
        public int Id { get; set; }
        public CorrespondenceFactType FactType { get; set; }

        public static FactTypeRecord Read(BinaryReader reader)
        {
            int factTypeId = reader.ReadInt32();
            string typeName = reader.ReadString();
            int version = reader.ReadInt32();

            return new FactTypeRecord
            {
                Id = factTypeId,
                FactType = new CorrespondenceFactType(typeName, version)
            };
        }

        public static void Write(BinaryWriter writer, FactTypeRecord record)
        {
            writer.Write(record.Id);
            writer.Write(record.FactType.TypeName);
            writer.Write(record.FactType.Version);
        }
    }
}
