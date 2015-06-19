using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;

namespace Correspondence.IsolatedStorage
{
    internal class PeerStore
    {
        private const string PeerTableFileName = "PeerTable.bin";
        private List<PeerRecord> _peerTable = new List<PeerRecord>();

        public int SavePeer(string protocolName, string peerName, IsolatedStorageFile store)
        {
            PeerRecord peerRecord = _peerTable.FirstOrDefault(peer =>
                peer.ProtocolName == protocolName &&
                peer.PeerName == peerName);
            if (peerRecord == null)
            {
                peerRecord = new PeerRecord
                {
                    PeerId = _peerTable.Count + 1,
                    ProtocolName = protocolName,
                    PeerName = peerName
                };
                _peerTable.Add(peerRecord);

                using (BinaryWriter writer = new BinaryWriter(
                    store.OpenFile(PeerTableFileName, FileMode.Append)))
                {
                    writer.Write(peerRecord.PeerId);
                    writer.Write(peerRecord.ProtocolName);
                    writer.Write(peerRecord.PeerName);
                }
            }

            return peerRecord.PeerId;
        }

        public static PeerStore Load(IsolatedStorageFile store)
        {
            PeerStore peerStore = new PeerStore();
            if (store.FileExists(PeerTableFileName))
            {
                using (BinaryReader peerReader = new BinaryReader(store.OpenFile(
                    PeerTableFileName,
                    FileMode.Open,
                    FileAccess.Read)))
                {
                    peerStore.ReadAllPeersFromStorage(peerReader);
                }
            }
            return peerStore;
        }

        public void ReadAllPeersFromStorage(BinaryReader peerReader)
        {
            long length = peerReader.BaseStream.Length;
            while (peerReader.BaseStream.Position < length)
            {
                ReadPeerFromStorage(peerReader);
            }
        }

        private void ReadPeerFromStorage(BinaryReader peerReader)
        {
            int peerId = peerReader.ReadInt32();
            string protocolName = peerReader.ReadString();
            string peerName = peerReader.ReadString();

            _peerTable.Add(
                new PeerRecord
                {
                    PeerId = peerId,
                    ProtocolName = protocolName,
                    PeerName = peerName
                });
        }

        public static void DeleteAll(IsolatedStorageFile store)
        {
            if (store.FileExists(PeerTableFileName))
                store.DeleteFile(PeerTableFileName);
        }
    }
}
