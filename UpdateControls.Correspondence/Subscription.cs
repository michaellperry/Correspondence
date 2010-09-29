using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Correspondence
{
    class Subscription
    {
        private List<CorrespondenceFact> _pivots;
        private Dependent _depPivots;

        public Subscription(Func<IEnumerable<CorrespondenceFact>> pivotsFunction)
        {
            _depPivots = new Dependent(() => _pivots = pivotsFunction().ToList());
        }

        public IEnumerable<CorrespondenceFact> Pivots
        {
            get { _depPivots.OnGet(); return _pivots; }
        }
    }
}
