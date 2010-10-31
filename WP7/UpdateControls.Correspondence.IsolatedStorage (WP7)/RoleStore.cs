using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.IsolatedStorage
{
    public class RoleStore
    {
        private const string RoleTableFileName = "RoleTable.bin";
        private IDictionary<int, RoleMemento> _roleById = new Dictionary<int, RoleMemento>();

        private RoleStore()
        {
        }

        public RoleMemento GetRole(int roleId)
        {
            RoleMemento roleMemento;
            if (!_roleById.TryGetValue(roleId, out roleMemento))
                throw new CorrespondenceException(String.Format("Role {0} is in tree, but is not recognized.", roleId));
            return roleMemento;
        }

        public int GetRoleId(RoleMemento role, IsolatedStorageFile store)
        {
            int roleId = _roleById
                .Where(pair => pair.Value.Equals(role))
                .Select(pair => pair.Key)
                .FirstOrDefault();
            if (roleId == 0)
            {
                roleId = _roleById.Count + 1;
                _roleById.Add(roleId, role);

                using (var writer = new BinaryWriter(store.OpenFile(RoleTableFileName, FileMode.Append)))
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

        public static RoleStore Load(IsolatedStorageFile store)
        {
            RoleStore roleStore = new RoleStore();
            if (store.FileExists(RoleTableFileName))
            {
                using (BinaryReader roleReader = new BinaryReader(store.OpenFile(
                    RoleTableFileName,
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

        public static void DeleteAll(IsolatedStorageFile store)
        {
            if (store.FileExists(RoleTableFileName))
                store.DeleteFile(RoleTableFileName);
        }
    }
}
