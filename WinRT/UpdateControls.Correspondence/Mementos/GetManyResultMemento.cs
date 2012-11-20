using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Mementos
{
    public class GetManyResultMemento
    {
        private readonly FactTreeMemento _factTree;
        private readonly List<PivotMemento> _pivots;

        public GetManyResultMemento(FactTreeMemento factTree, List<PivotMemento> pivots)
        {
            _factTree = factTree;
            _pivots = pivots;
        }

        public FactTreeMemento MessageBody
        {
            get { return _factTree; }
        }

        public IEnumerable<PivotMemento> NewTimestamps
        {
            get { return _pivots; }
        }
    }
}
