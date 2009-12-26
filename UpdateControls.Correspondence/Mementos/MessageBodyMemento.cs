using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Strategy;

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

        public FactID PivotId
        {
            get { return _pivotId; }
        }

        public IEnumerable<IdentifiedFactMemento> Facts
        {
            get { return _facts; }
        }

        public bool Contains(FactID factId)
        {
            return _facts.Any(f => f.Id.Equals(factId));
        }

        public void Add(IdentifiedFactMemento identifiedFact)
        {
            _facts.Add(identifiedFact);
        }
    }
}
