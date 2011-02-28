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
    }
}
