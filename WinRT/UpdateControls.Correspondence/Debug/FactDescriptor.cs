using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Debug
{
    public class FactDescriptor
    {
        private readonly FactID _id;
        private readonly FactMemento _fact;
        private readonly Func<FactID, FactMemento> _loadFact;
        private readonly Func<FactID, List<IdentifiedFactMemento>> _loadSuccessors;

        public FactDescriptor(
            FactID id,
            FactMemento fact,
            Func<FactID, FactMemento> loadFact,
            Func<FactID, List<IdentifiedFactMemento>> loadSuccessors)
        {
            _id = id;
            _fact = fact;
            _loadFact = loadFact;
            _loadSuccessors = loadSuccessors;
        }

        public string Data
        {
            get
            {
                if (_fact.Data == null)
                    return null;
                var chars =
                    from b in _fact.Data
                    select (b < 0x20 || b > 0x7E) ? '.' : (char)b;
                return new String(chars.ToArray());
            }
        }

        public PredecessorDescriptor[] Predecessors
        {
            get
            {
                return
                    (from predecessor in _fact.Predecessors
                     select new PredecessorDescriptor(predecessor, _loadFact, _loadSuccessors))
                    .ToArray();
            }
        }

        public IEnumerable<FactDescriptor> Successors
        {
            get
            {
                foreach (IdentifiedFactMemento successor in _loadSuccessors(_id))
                    yield return new FactDescriptor(successor.Id, successor.Memento, _loadFact, _loadSuccessors);
            }
        }

        public override string ToString()
        {
            return String.Format("{0} : {1}", _fact.FactType, _id);
        }
    }
}
