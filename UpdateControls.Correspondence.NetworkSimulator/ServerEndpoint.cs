using System;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class ServerEndpoint
    {
        private Type _factType;
        private Func<IMessageRepository, string, CorrespondenceFact> _pathToFact;

        public ServerEndpoint(Type factType, Func<IMessageRepository, string, CorrespondenceFact> pathToFact)
        {
            _factType = factType;
            _pathToFact = pathToFact;
        }

        public CorrespondenceFact GetFact(IMessageRepository repository, string path)
        {
            return _pathToFact(repository, path);
        }
    }
}
