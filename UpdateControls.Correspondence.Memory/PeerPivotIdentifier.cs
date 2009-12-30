using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Memory
{
    public class PeerPivotIdentifier
    {
        private PeerIdentifier _peerIdentifier;
        private FactID _pivotId;

        public PeerPivotIdentifier(PeerIdentifier peerIdentifier, FactID pivotId)
        {
            _peerIdentifier = peerIdentifier;
            _pivotId = pivotId;
        }

        public PeerIdentifier PeerIdentifier
        {
            get { return _peerIdentifier; }
        }

        public FactID PivotId
        {
            get { return _pivotId; }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            PeerPivotIdentifier that = obj as PeerPivotIdentifier;
            if (that == null)
                return false;
            return
                object.Equals(this.PeerIdentifier, that.PeerIdentifier) &&
                object.Equals(this.PivotId, that.PivotId);
        }

        public override int GetHashCode()
        {
            return _peerIdentifier.GetHashCode() * 37 + _pivotId.GetHashCode();
        }
    }
}
