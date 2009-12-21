using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence
{
    public abstract class RoleBase
    {
        protected RoleMemento _roleMemento;

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
