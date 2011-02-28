using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence
{
    public abstract class RoleBase
    {
        private RoleMemento _roleMemento;

        protected RoleBase(RoleMemento roleMemento)
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
