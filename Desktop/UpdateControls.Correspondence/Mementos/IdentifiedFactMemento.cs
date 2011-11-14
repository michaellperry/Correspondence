using System;

namespace UpdateControls.Correspondence.Mementos
{
    public class IdentifiedFactMemento
    {
        private FactID _id;
        private FactMemento _memento;

        public IdentifiedFactMemento(FactID id, FactMemento memento)
        {
            _id = id;
            _memento = memento;
        }

        public FactID Id
        {
            get { return _id; }
        }

        public FactMemento Memento
        {
            get { return _memento; }
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode() * 37 + _memento.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            IdentifiedFactMemento that = obj as IdentifiedFactMemento;
            if (that == null)
                return false;
            return _id.Equals(that._id) && _memento.Equals(that._memento);
        }
    }
}
