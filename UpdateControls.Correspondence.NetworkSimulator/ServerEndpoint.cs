using System;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class ServerEndpoint
    {
        private Type _factType;
        private Func<IMessageRepository, string, CorrespondenceFact> _urlToFact;

        public ServerEndpoint(Type factType, Func<IMessageRepository, string, CorrespondenceFact> urlToFact)
        {
            _factType = factType;
            _urlToFact = urlToFact;
        }

        public CorrespondenceFact GetFact(IMessageRepository repository, string url)
        {
            return _urlToFact(repository, url);
        }
    }
}
