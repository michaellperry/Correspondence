using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Debug
{
    public class PredecessorDescriptor
    {
        private readonly PredecessorMemento _predecessor;
        private readonly Func<FactID, FactMemento> _loadFact;
        private readonly Func<FactID, List<IdentifiedFactMemento>> _loadSuccessors;

        public PredecessorDescriptor(
            PredecessorMemento predecessor,
            Func<FactID, FactMemento> loadFact,
            Func<FactID, List<IdentifiedFactMemento>> loadSuccessors)
        {
            _predecessor = predecessor;
            _loadFact = loadFact;
            _loadSuccessors = loadSuccessors;
        }

        public bool IsPublishedTo
        {
            get { return _predecessor.IsPivot; }
        }

        public FactDescriptor Fact
        {
            get
            {
                return new FactDescriptor(
                    _predecessor.ID, 
                    _loadFact(_predecessor.ID), 
                    _loadFact, 
                    _loadSuccessors);
            }
        }

        public override string ToString()
        {
            return _predecessor.ToString();
        }
    }
}
