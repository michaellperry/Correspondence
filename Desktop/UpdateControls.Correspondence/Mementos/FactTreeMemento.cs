using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Correspondence.Mementos
{
    public class FactTreeMemento
    {
        private long _databaseId;
        private List<IdentifiedFactMemento> _facts = new List<IdentifiedFactMemento>();

        public FactTreeMemento(long databaseId)
        {
            _databaseId = databaseId;
        }

        public long DatabaseId
        {
            get { return _databaseId; }
        }

        public IEnumerable<IdentifiedFactMemento> Facts
        {
            get { return _facts; }
        }

        public bool Contains(FactID factId)
        {
            return _facts.Any(f => f.Id.Equals(factId));
        }

        public IdentifiedFactMemento Get(FactID factId)
        {
            return _facts.FirstOrDefault(f => f.Id.Equals(factId));
        }

        public void Add(IdentifiedFactMemento identifiedFact)
        {
            _facts.Add(identifiedFact);
        }
    }
}
