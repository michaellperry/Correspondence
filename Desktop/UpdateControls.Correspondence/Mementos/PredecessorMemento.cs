
namespace UpdateControls.Correspondence.Mementos
{
    public class PredecessorMemento
    {
        private RoleMemento _role;
        private FactID _id;

        public PredecessorMemento(RoleMemento role, FactID id)
        {
            _role = role;
            _id = id;
        }

        public RoleMemento Role
        {
            get { return _role; }
        }

        public FactID ID
        {
            get { return _id; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            PredecessorMemento that = (PredecessorMemento)obj;
            return
                this._role.Equals(that.Role) &&
                this._id.Equals(that._id);
        }

        public override int GetHashCode()
        {
            return _role.GetHashCode() ^ _id.GetHashCode();
        }

        public override string ToString()
        {
            return _role.ToString() + "=" + _id.ToString();
        }
    }
}
