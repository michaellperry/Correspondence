using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class ClientGetEndpoint
    {
        private Type _factType;
        private Func<IEnumerable<CorrespondenceFact>> _pivots;

        public ClientGetEndpoint(Type factType, Func<IEnumerable<CorrespondenceFact>> pivots)
        {
            _factType = factType;
            _pivots = pivots;
        }

        public bool IsCompatibleWith(CorrespondenceFact fact)
        {
            return _factType.IsAssignableFrom(fact.GetType());
        }

        public IEnumerable<CorrespondenceFact> Pivots
        {
            get { return _pivots(); }
        }
    }
}
