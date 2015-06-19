using System;

namespace Correspondence.Mementos
{
    public class IdentifiedFactMemento : IdentifiedFactBase
    {
        private readonly FactMemento _memento;

        public IdentifiedFactMemento(FactID id, FactMemento memento) :
            base(id)
        {
            _memento = memento;
        }

        public FactMemento Memento
        {
            get { return _memento; }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() * 37 + _memento.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            IdentifiedFactMemento that = obj as IdentifiedFactMemento;
            if (that == null)
                return false;
            return Id.Equals(that.Id) && _memento.Equals(that._memento);
        }
    }
}
