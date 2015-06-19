using Correspondence.Mementos;

namespace Correspondence
{
    public class Role
    {
        private RoleMemento _roleMemento;

        public Role(RoleMemento roleMemento)
        {
            _roleMemento = roleMemento;
        }

        internal RoleMemento RoleMemento
        {
            get { return _roleMemento; }
        }

        public override string ToString()
        {
            return _roleMemento.ToString();
        }
    }
}
