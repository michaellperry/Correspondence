using System;

namespace UpdateControls.Correspondence.FileStream
{
    internal class PeerIdentifier
    {
        private string _protocolName;
        private string _peerName;

        public PeerIdentifier(string protocolName, string peerName)
        {
            _protocolName = protocolName;
            _peerName = peerName;
        }

        public string ProtocolName
        {
            get { return _protocolName; }
        }

        public string PeerName
        {
            get { return _peerName; }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            PeerIdentifier that = obj as PeerIdentifier;
            if (that == null)
                return false;
            return
                Object.Equals(this.ProtocolName, that.ProtocolName) &&
                Object.Equals(this.PeerName, that.PeerName);
        }

        public override int GetHashCode()
        {
            return ProtocolName.GetHashCode() * 37
                + PeerName.GetHashCode();
        }
    }
}
