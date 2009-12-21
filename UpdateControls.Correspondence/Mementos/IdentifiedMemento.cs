
namespace UpdateControls.Correspondence.Mementos
{
    public class IdentifiedMemento
    {
        private FactID _id;
        private Memento _memento;

        public IdentifiedMemento(FactID id, Memento memento)
        {
            _id = id;
            _memento = memento;
        }

        public FactID Id
        {
            get { return _id; }
        }

        public Memento Memento
        {
            get { return _memento; }
        }
    }
}
