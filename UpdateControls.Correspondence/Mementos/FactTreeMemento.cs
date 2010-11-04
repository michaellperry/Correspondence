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
            long databaseId = _databaseId;
            long timestamp = _timestamp;
            if (inputTree._databaseId > _databaseId ||
                inputTree._databaseId == _databaseId && inputTree._timestamp > _timestamp)
            {
                databaseId = inputTree._databaseId;
                timestamp = inputTree._timestamp;
            }

            FactTreeMemento merged = new FactTreeMemento(databaseId, timestamp);
            foreach (IdentifiedFactMemento firstTreeFact in this.Facts)
            {
                if (!merged.Contains(firstTreeFact.Id))
                    merged.Add(firstTreeFact);
            }
            foreach (IdentifiedFactMemento inputTreeFact in inputTree.Facts)
            {
                if (!merged.Contains(inputTreeFact.Id))
                    merged.Add(inputTreeFact);
            }
            return merged;
        }
    }
}
