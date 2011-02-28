using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Correspondence
{
    class Subscription
    {
        private Community _community;
        private Func<IEnumerable<CorrespondenceFact>> _pivotsFunction;
        private List<CorrespondenceFact> _pivots;
        private Dependent _depPivots;

        public Subscription(Community community, Func<IEnumerable<CorrespondenceFact>> pivotsFunction)
        {
            _community = community;
            _pivotsFunction = pivotsFunction;
            _depPivots = new Dependent(UpdatePivots);
        }

        public IEnumerable<CorrespondenceFact> Pivots
        {
            get { _depPivots.OnGet(); return _pivots; }
        }

        private void UpdatePivots()
        {
            _pivots = _pivotsFunction().ToList();

            if (_pivots.Any(pivot => pivot.InternalCommunity != _community))
                throw new CorrespondenceException("The facts must come from the same community as the subscription.");
        }
    }
}
