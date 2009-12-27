using System;
using System.Collections.Generic;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class SimulatedServer : SimulatedMachine
    {
        private SimulatedNetwork _network;

        private List<ServerEndpoint> _postEndpoints = new List<ServerEndpoint>();
        private List<ServerEndpoint> _getEndpoints = new List<ServerEndpoint>();

        public SimulatedServer(SimulatedNetwork network)
        {
            _network = network;

            network.AttachServer(this);
        }

        public SimulatedServer Post<TFact>(Func<IMessageRepository, string, TFact> pathToFact)
            where TFact : CorrespondenceFact
        {
            _postEndpoints.Add(new ServerEndpoint(typeof(TFact), (repository, path) => pathToFact(repository, path)));
            return this;
        }

        public SimulatedServer Get<TFact>(Func<IMessageRepository, string, TFact> pathToFact)
            where TFact : CorrespondenceFact
        {
            _getEndpoints.Add(new ServerEndpoint(typeof(TFact), (repository, path) => pathToFact(repository, path)));
            return this;
        }

        public void Receive(string path, MessageBodyMemento messageBody)
        {
            foreach (ServerEndpoint postEndpoint in _postEndpoints)
            {
                CorrespondenceFact pivot = postEndpoint.GetFact(_repository, path);
                if (pivot != null)
                {
                    ReceiveMessage(messageBody, pivot);
                }
            }
        }

        public MessageBodyMemento Get(string path, TimestampID timestamp)
        {
            foreach (ServerEndpoint getEndpoint in _getEndpoints)
            {
                CorrespondenceFact pivot = getEndpoint.GetFact(_repository, path);
                if (pivot != null)
                {
                    FactID pivotId = _repository.IDOfFact(pivot);
                    IEnumerable<FactID> recentMessages = _repository.LoadRecentMessages(pivotId, timestamp);
                    MessageBodyMemento messageBody = new MessageBodyMemento(pivotId);
                    foreach (FactID recentMessage in recentMessages)
                        AddFactTree(messageBody, recentMessage);

                    return messageBody;
                }
            }

            throw new NetworkSimulatorException(string.Format("No server Get found for path {0}.", path));
        }
    }
}
