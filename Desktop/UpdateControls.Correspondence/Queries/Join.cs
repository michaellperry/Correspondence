using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Queries
{
    public class Join
    {
        private bool _successor;
        private RoleMemento _role;
        private Condition _condition;

        public Join(bool successor, RoleMemento role, Condition condition)
        {
            _successor = successor;
            _role = role;
            _condition = condition;
        }

        public bool Successor
        {
            get { return _successor; }
        }

        public RoleMemento Role
        {
            get { return _role; }
        }

        public Condition Condition
        {
            get { return _condition; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            Join that = (Join)obj;
            return
                this._role.Equals(that._role) &&
                this._successor == that._successor &&
				object.Equals(this._condition, that._condition);
        }

        public override int GetHashCode()
        {
			int hash = _role.GetHashCode() * 2 + (_successor ? 1 : 0);
            if (_condition != null)
				hash = hash * 37 + _condition.GetHashCode();
			return hash;
        }

		public string ToString(string prior)
		{
			StringBuilder result = new StringBuilder();
            string name;
			if (_successor)
			{
				string type = _role.DeclaringType.TypeName.Split('.').Last();
				name = type.ToLower();
				result.AppendFormat("{0} {1}: {1}.{2} = {3}", type, name, _role.RoleName, prior);
			}
			else
			{
				string type = _role.TargetType.TypeName.Split('.').Last();
				name = type.ToLower();
				result.AppendFormat("{0} {1}: {3}.{2} = {1}", type, name, _role.RoleName, prior);
			}

			if (_condition != null)
                result.AppendFormat(" and {0}", _condition.ToString(name));

            return result.ToString();
		}

		public override string ToString()
		{
			return ToString("this");
		}
    }
}
