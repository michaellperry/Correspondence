
namespace Correspondence.Mementos
{
    public class UnpublishMemento
    {
        private FactID _messageId;
        private RoleMemento _role;

        public UnpublishMemento(FactID messageId, RoleMemento role)
        {
            _role = role;
            _messageId = messageId;
        }

        public RoleMemento Role
        {
            get { return _role; }
        }

        public FactID MessageId
        {
            get { return _messageId; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            UnpublishMemento that = (UnpublishMemento)obj;
            return
                this._role.Equals(that.Role) &&
                this._messageId.Equals(that._messageId);
        }

        public override int GetHashCode()
        {
            return _role.GetHashCode() * 37 + _messageId.GetHashCode();
        }

        public override string ToString()
        {
            return _role.ToString() + "=" + _messageId.ToString();
        }
    }
}
