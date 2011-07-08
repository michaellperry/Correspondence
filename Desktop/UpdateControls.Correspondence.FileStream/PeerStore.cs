using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UpdateControls.Correspondence.FileStream
{
    internal class PeerStore
    {
        private const string PeerTableFileName = "PeerTable.bin";
        private string _fileName;
        private List<PeerRecord> _peerTable = new List<PeerRecord>();

        public PeerStore(string fileName)
        {
            _fileName = fileName;
        }

        public int SavePeer(string protocolName, string peerName)
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
                    File.Open(_fileName, FileMode.Append)))
                {
                    writer.Write(peerRecord.PeerId);
                    writer.Write(peerRecord.ProtocolName);
                    writer.Write(peerRecord.PeerName);
                }
            }

            return peerRecord.PeerId;
        }

        public static PeerStore Load(string filePath)
        {
            string fileName = Path.Combine(filePath, PeerTableFileName);
            PeerStore peerStore = new PeerStore(fileName);
            if (File.Exists(fileName))
            {
                using (BinaryReader peerReader = new BinaryReader(File.Open(
                    fileName,
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

        public static void DeleteAll(string filePath)
        {
            string fileName = Path.Combine(filePath, PeerTableFileName);
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }
}
