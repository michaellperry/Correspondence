using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;
using System.IO;

namespace UpdateControls.Correspondence.FileStream
{
    internal class RoleRecord
    {
        public int Id { get; set; }
        public RoleMemento Role { get; set; }

        public static RoleRecord Read(BinaryReader reader)
        {
            int roleId = reader.ReadInt32();
            string declaringTypeName = reader.ReadString();
            int declaringTypeVersion = reader.ReadInt32();
            string roleName = reader.ReadString();
            string targetTypeName = reader.ReadString();
            int targetTypeVersion = reader.ReadInt32();
            bool isPivot = reader.ReadByte() != 0;

            return new RoleRecord
            {
                Id = roleId,
                Role = new RoleMemento(
                    new CorrespondenceFactType(declaringTypeName, declaringTypeVersion),
                    roleName,
                    new CorrespondenceFactType(targetTypeName, targetTypeVersion),
                    isPivot)
            };
        }

        public static void Write(BinaryWriter writer, RoleRecord record)
        {
            writer.Write(record.Id);
            writer.Write(record.Role.DeclaringType.TypeName);
            writer.Write(record.Role.DeclaringType.Version);
            writer.Write(record.Role.RoleName);
            writer.Write(record.Role.TargetType == null ? string.Empty : record.Role.TargetType.TypeName);
            writer.Write(record.Role.TargetType == null ? 0 : record.Role.TargetType.Version);
            writer.Write(record.Role.IsPivot ? (byte)1 : (byte)0);
        }
    }
}
