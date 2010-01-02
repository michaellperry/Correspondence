using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Correspondence.Mementos
{
    public class FactTreeMemento
    {
        private List<IdentifiedFactMemento> _facts = new List<IdentifiedFactMemento>();

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
