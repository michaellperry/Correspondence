using System.IO;

namespace UpdateControls.Correspondence.FileStream
{
    internal class PeerRecord
    {
        public int PeerId;
        public string ProtocolName;
        public string PeerName;

        public static PeerRecord Read(BinaryReader reader)
        {
            int peerId = reader.ReadInt32();
            string protocolName = reader.ReadString();
            string peerName = reader.ReadString();

            return new PeerRecord
            {
                PeerId = peerId,
                ProtocolName = protocolName,
                PeerName = peerName
            };
        }

        public static void Write(BinaryWriter writer, PeerRecord record)
        {
            writer.Write(record.PeerId);
            writer.Write(record.ProtocolName);
            writer.Write(record.PeerName);
        }
    }
}
