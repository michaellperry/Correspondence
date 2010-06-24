using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence
{
    class Subscription
    {
        private Func<IEnumerable<CorrespondenceFact>> _pivots;

        public Subscription(Func<IEnumerable<CorrespondenceFact>> pivots)
        {
            _pivots = pivots;
        }

        public IEnumerable<CorrespondenceFact> Pivots
        {
            get { return _pivots(); }
        }
    }
}
