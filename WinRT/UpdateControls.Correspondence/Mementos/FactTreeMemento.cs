using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Correspondence.Mementos
{
    public class FactTreeMemento
    {
        private long _databaseId;
        private List<IdentifiedFactBase> _facts = new List<IdentifiedFactBase>();

        public FactTreeMemento(long databaseId)
        {
            _databaseId = databaseId;
        }

        public long DatabaseId
        {
            get { return _databaseId; }
        }

        public IEnumerable<IdentifiedFactBase> Facts
        {
            get { return _facts; }
        }

        public bool Contains(FactID factId)
        {
            return _facts.Any(f => f.Id.Equals(factId));
        }

        public IdentifiedFactBase Get(FactID factId)
        {
            return _facts.FirstOrDefault(f => f.Id.Equals(factId));
        }

        public FactTreeMemento Add(IdentifiedFactBase identifiedFact)
        {
            _facts.Add(identifiedFact);
            return this;
        }

        public override int GetHashCode()
        {
            int hashCode = _databaseId.GetHashCode();
            foreach (var fact in _facts)
                hashCode += fact.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            FactTreeMemento that = obj as FactTreeMemento;
            if (that == null)
                return false;
            return _databaseId == that._databaseId
                && _facts.Count == that._facts.Count
                && _facts.All(f => that._facts.Contains(f));
        }
    }
}
