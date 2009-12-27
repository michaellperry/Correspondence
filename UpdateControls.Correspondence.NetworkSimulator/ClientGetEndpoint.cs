using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class ClientGetEndpoint : ClientEndpoint
    {
        private Func<IEnumerable<CorrespondenceFact>> _pivots;

        public ClientGetEndpoint(Type factType, Func<IEnumerable<CorrespondenceFact>> pivots, Func<CorrespondenceFact, string> factToPath)
            : base(factType, factToPath)
        {
            _pivots = pivots;
        }

        public IEnumerable<CorrespondenceFact> Pivots
        {
            get { return _pivots(); }
        }
    }
}
