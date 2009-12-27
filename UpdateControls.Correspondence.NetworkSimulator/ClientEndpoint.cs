using System;
using UpdateControls.Correspondence;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public abstract class ClientEndpoint
    {
        private Type _factType;
        private Func<CorrespondenceFact, string> _factToPath;

        public ClientEndpoint(Type factType, Func<CorrespondenceFact, string> factToPath)
        {
            _factType = factType;
            _factToPath = factToPath;
        }

        public bool IsCompatibleWith(CorrespondenceFact fact)
        {
            return _factType.IsAssignableFrom(fact.GetType());
        }

        public string GetPath(CorrespondenceFact fact)
        {
            return _factToPath(fact);
        }
    }
}
