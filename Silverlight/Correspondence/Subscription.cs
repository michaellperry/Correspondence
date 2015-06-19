using Assisticant;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Correspondence
{
    class Subscription
    {
        private Community _community;
        private Func<IEnumerable<CorrespondenceFact>> _pivotsFunction;
        private List<CorrespondenceFact> _pivots;
        private Computed _depPivots;

        public Subscription(Community community, Func<IEnumerable<CorrespondenceFact>> pivotsFunction)
        {
            _community = community;
            _pivotsFunction = pivotsFunction;
            _depPivots = new Computed(UpdatePivots);
        }

        public IEnumerable<CorrespondenceFact> Pivots
        {
            get { _depPivots.OnGet(); return _pivots; }
        }

        private void UpdatePivots()
        {
            IEnumerable<CorrespondenceFact> pivots = _pivotsFunction() ??
                Enumerable.Empty<CorrespondenceFact>();
            _pivots = pivots.ToList();

            if (_pivots.Any(pivot => pivot.InternalCommunity != _community))
                throw new CorrespondenceException("The facts must come from the same community as the subscription.");
        }
    }
}
