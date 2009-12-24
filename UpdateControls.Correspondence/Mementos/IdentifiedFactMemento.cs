using System.Collections.Generic;
using System;
using System.Linq;

namespace UpdateControls.Correspondence.Mementos
{
    public class MessageBodyMemento
    {
        private FactID _pivotId;
        private List<IdentifiedFactMemento> _facts = new List<IdentifiedFactMemento>();

        public MessageBodyMemento(FactID pivotId)
        {
            _pivotId = pivotId;
        }

        public bool Add(FactID factId, FactMemento fact)
        {
            if (!_facts.Any(f => f.Id.Equals(factId)))
            {
                _facts.Add(new IdentifiedFactMemento(factId, fact));
                return true;
            }
            return false;
        }
    }
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
