using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Correspondence.Mementos;

namespace Correspondence.Memory
{
    public class PeerPivotIdentifier
    {
        private int _peerId;
        private FactID _pivotId;

        public PeerPivotIdentifier(int peerId, FactID pivotId)
        {
            _peerId = peerId;
            _pivotId = pivotId;
        }

        public int PeerId
        {
            get { return _peerId; }
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
                object.Equals(this.PeerId, that.PeerId) &&
                object.Equals(this.PivotId, that.PivotId);
        }

        public override int GetHashCode()
        {
            return _peerId * 37 + _pivotId.GetHashCode();
        }
    }
}
