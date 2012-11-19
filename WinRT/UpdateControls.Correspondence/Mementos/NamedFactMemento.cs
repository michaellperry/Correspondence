
namespace UpdateControls.Correspondence.Mementos
{
    public class NamedFactMemento
    {
        private string _name;
        private FactID _factId;

        public NamedFactMemento(string name, FactID factId)
        {
            _name = name;
            _factId = factId;
        }

        public string Name
        {
            get { return _name; }
        }

        public FactID FactId
        {
            get { return _factId; }
        }
    }
}
