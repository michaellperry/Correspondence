using System;

namespace UpdateControls.Correspondence.Mementos
{
    public class IdentifiedFactRemote : IdentifiedFactBase
    {
        private readonly FactID _remoteId;

        public IdentifiedFactRemote(FactID id, FactID remoteId)
            : base(id)
        {
            _remoteId = remoteId;
        }

        public FactID RemoteId
        {
            get { return _remoteId; }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            IdentifiedFactRemote that = obj as IdentifiedFactRemote;
            if (that == null)
                return false;
            return this.Id.Equals(that.Id) && this._remoteId.Equals(that._remoteId);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() * 37 + _remoteId.GetHashCode();
        }
    }
}
