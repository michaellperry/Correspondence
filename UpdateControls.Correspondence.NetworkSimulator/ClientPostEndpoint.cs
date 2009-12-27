using System;
using UpdateControls.Correspondence;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class ClientPostEndpoint : ClientEndpoint
    {
        public ClientPostEndpoint(Type factType, Func<CorrespondenceFact, string> factToPath)
            : base(factType, factToPath)
        {
        }
    }
}
