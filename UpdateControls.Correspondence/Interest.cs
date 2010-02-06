using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence
{
    class Interest
    {
        private Func<IEnumerable<CorrespondenceFact>> _pivots;

        public Interest(Func<IEnumerable<CorrespondenceFact>> pivots)
        {
            _pivots = pivots;
        }

        public IEnumerable<CorrespondenceFact> Pivots
        {
            get { return _pivots(); }
        }
    }
}
