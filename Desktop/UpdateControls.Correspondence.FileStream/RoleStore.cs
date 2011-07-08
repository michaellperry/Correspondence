using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.FileStream
{
    public class RoleStore
    {
        private const string RoleTableFileName = "RoleTable.bin";
        private string _fileName;
        private IDictionary<int, RoleMemento> _roleById = new Dictionary<int, RoleMemento>();

        public RoleStore(string fileName)
        {
            _fileName = fileName;
        }

        public RoleMemento GetRole(int roleId)
        {
            RoleMemento roleMemento;
            if (!_roleById.TryGetValue(roleId, out roleMemento))
                throw new CorrespondenceException(String.Format("Role {0} is in tree, but is not recognized.", roleId));
            return roleMemento;
        }

        public int GetRoleId(RoleMemento role)
        {
            int roleId = _roleById
                .Where(pair => pair.Value.Equals(role))
                .Select(pair => pair.Key)
                .FirstOrDefault();
            if (roleId == 0)
            {
                roleId = _roleById.Count + 1;
                _roleById.Add(roleId, role);

                using (var writer = new BinaryWriter(File.Open(_fileName, FileMode.Append)))
                {
                    writer.Write(roleId);
                    writer.Write(role.DeclaringType.TypeName);
                    writer.Write(role.DeclaringType.Version);
                    writer.Write(role.RoleName);
                    writer.Write(role.TargetType.TypeName);
                    writer.Write(role.TargetType.Version);
                    writer.Write(role.IsPivot ? (byte)1 : (byte)0);
                }
            }
            return roleId;
        }

        public static RoleStore Load(string filePath)
        {
            string fileName = Path.Combine(filePath, RoleTableFileName);
            RoleStore roleStore = new RoleStore(fileName);
            if (File.Exists(fileName))
            {
                using (BinaryReader roleReader = new BinaryReader(File.Open(
                    fileName,
                    FileMode.Open,
                    FileAccess.Read)))
                {
                    roleStore.ReadAllRolesFromStorage(roleReader);
                }
            }
            return roleStore;
        }

        private void ReadAllRolesFromStorage(BinaryReader roleReader)
        {
            long length = roleReader.BaseStream.Length;
            while (roleReader.BaseStream.Position < length)
            {
                ReadRoleFromStorage(roleReader);
            }
        }

        private void ReadRoleFromStorage(BinaryReader roleReader)
        {
            int roleId = roleReader.ReadInt32();
            string declaringTypeName = roleReader.ReadString();
            int declaringTypeVersion = roleReader.ReadInt32();
            string roleName = roleReader.ReadString();
            string targetTypeName = roleReader.ReadString();
            int targetTypeVersion = roleReader.ReadInt32();
            bool isPivot = roleReader.ReadByte() != 0;

            _roleById.Add(
                roleId,
                new RoleMemento(
                    new CorrespondenceFactType(declaringTypeName, declaringTypeVersion),
                    roleName,
                    new CorrespondenceFactType(targetTypeName, targetTypeVersion),
                    isPivot));
        }

        public static void DeleteAll(string filePath)
        {
            string fileName = Path.Combine(filePath, RoleTableFileName);
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }
}
