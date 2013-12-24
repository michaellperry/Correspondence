
namespace UpdateControls.Correspondence.Mementos
{
    public class GetResultMemento
    {
        private FactTreeMemento _factTree;
        private TimestampID _timestamp;

        public GetResultMemento(FactTreeMemento factTree, TimestampID timestamp)
        {
            _factTree = factTree;
            _timestamp = timestamp;
        }

        public FactTreeMemento FactTree
        {
            get { return _factTree; }
        }

        public TimestampID Timestamp
        {
            get { return _timestamp; }
        }
    }
}
