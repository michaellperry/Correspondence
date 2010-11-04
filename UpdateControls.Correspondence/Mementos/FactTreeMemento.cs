using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Correspondence.Mementos
{
    public class FactTreeMemento
    {
        private long _databaseId;
        private List<IdentifiedFactMemento> _facts = new List<IdentifiedFactMemento>();
        private long _timestamp;

        public FactTreeMemento(long databaseId, long timestamp)
        {
            _databaseId = databaseId;
            _timestamp = timestamp;
        }

        public long DatabaseId
        {
            get { return _databaseId; }
        }

        public IEnumerable<IdentifiedFactMemento> Facts
        {
            get { return _facts; }
        }

        public long Timestamp
        {
            get { return _timestamp; }
        }

        public bool Contains(FactID factId)
        {
            return _facts.Any(f => f.Id.Equals(factId));
        }

        public void Add(IdentifiedFactMemento identifiedFact)
        {
            _facts.Add(identifiedFact);
        }

        public FactTreeMemento Merge(FactTreeMemento inputTree)
        {
            FactTreeMemento merged = new FactTreeMemento(0,0);
            foreach (IdentifiedFactMemento firstTreeFact in this.Facts)
            {
                merged.Add(firstTreeFact);
            }
            IEnumerable<IdentifiedFactMemento> inputTreeFacts = inputTree.Facts;
            foreach (IdentifiedFactMemento inputTreeFact in inputTreeFacts)
            {
                merged.Add(inputTreeFact);
            }
            return merged;
        }
    }
}
