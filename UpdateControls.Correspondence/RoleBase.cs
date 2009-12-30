using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence
{
    public abstract class RoleBase
    {
        private RoleMemento _roleMemento;
        private RoleRelationship _metadata;

        protected RoleBase(RoleMemento roleMemento, RoleRelationship metadata)
        {
            _roleMemento = roleMemento;
            _metadata = metadata;
        }

        internal RoleMemento RoleMemento
        {
            get { return _roleMemento; }
        }

        internal RoleRelationship Metadata
        {
            get { return _metadata; }
        }

        public override string ToString()
        {
            return _roleMemento.ToString();
        }
    }
}
