using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence;
using System;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class ClientEndpoint
    {
        private Type _factType;
        private Func<CorrespondenceFact, string> _factToUrl;

        public ClientEndpoint(Type factType, Func<CorrespondenceFact, string> factToUrl)
        {
            _factType = factType;
            _factToUrl = factToUrl;
        }

        public bool IsCompatibleWith(CorrespondenceFact fact)
        {
            return _factType.IsAssignableFrom(fact.GetType());
        }

        public string GetUrl(CorrespondenceFact fact)
        {
            return _factToUrl(fact);
        }
    }
}
