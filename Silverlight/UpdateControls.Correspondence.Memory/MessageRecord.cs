using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Memory
{
    public class MessageRecord
    {
        private MessageMemento _message;
        private FactID _ancestorFact;
        private RoleMemento _ancestorRole;

        public MessageRecord(MessageMemento message, FactID ancestorFact, RoleMemento ancestorRole)
        {
            _message = message;
            _ancestorFact = ancestorFact;
            _ancestorRole = ancestorRole;
        }

        public MessageMemento Message
        {
            get { return _message; }
        }

        public FactID AncestorFact
        {
            get { return _ancestorFact; }
        }

        public RoleMemento AncestorRole
        {
            get { return _ancestorRole; }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            MessageRecord that = obj as MessageRecord;
            if (that == null)
                return false;
            return
                this._message.Equals(that._message) &&
                this._ancestorFact.Equals(that._ancestorFact) &&
                this._ancestorRole.Equals(that._ancestorRole);
        }

        public override int GetHashCode()
        {
            return
                _message.GetHashCode() * 967 +
                _ancestorFact.GetHashCode() * 31 +
                _ancestorRole.GetHashCode();
        }
    }
}
