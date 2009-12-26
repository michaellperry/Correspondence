using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class SimulatedClient : ICommunicationStrategy
    {
        private const string PROTOCOL_NAME = "simulator";
        private const string PEER_NAME = "client";

        private SimulatedNetwork _network;
        private IMessageRepository _repository;
        private List<ClientEndpoint> _postEndpoints = new List<ClientEndpoint>();

        public SimulatedClient(SimulatedNetwork network)
        {
            _network = network;

            network.AttachClient(this);
        }

        public SimulatedClient Post<FactType>(Func<FactType, string> factToUrl)
            where FactType : CorrespondenceFact
        {
            _postEndpoints.Add(new ClientEndpoint(typeof(FactType), fact => factToUrl((FactType)fact)));
            return this;
        }

        public void AttachMessageRepository(IMessageRepository repository)
        {
            _repository = repository;
        }

        public void Synchronize()
        {
            TimestampID timestamp = _repository.LoadTimestamp(PROTOCOL_NAME, PEER_NAME);
            IEnumerable<MessageBodyMemento> messageBodies = GetMessageBodies(ref timestamp);
            SendMessageBodiesToServer(messageBodies);
            _repository.SaveTimestamp(PROTOCOL_NAME, PEER_NAME, timestamp);
        }

        private IEnumerable<MessageBodyMemento> GetMessageBodies(ref TimestampID timestamp)
        {
            IDictionary<FactID, MessageBodyMemento> messageBodiesByPivotId = new Dictionary<FactID, MessageBodyMemento>();
            IEnumerable<MessageMemento> recentMessages = _repository.LoadRecentMessages(ref timestamp);
            foreach (MessageMemento message in recentMessages)
            {
                MessageBodyMemento messageBody;
                if (!messageBodiesByPivotId.TryGetValue(message.PivotId, out messageBody))
                {
                    messageBody = new MessageBodyMemento(message.PivotId);
                    messageBodiesByPivotId.Add(message.PivotId, messageBody);
                }
                AddFactTree(messageBody, message.FactId);
            }
            return messageBodiesByPivotId.Values;
        }

        private void AddFactTree(MessageBodyMemento messageBody, FactID factId)
        {
            if (!factId.Equals(messageBody.PivotId) && !messageBody.Contains(factId))
            {
                FactMemento fact = _repository.LoadFact(factId);
                foreach (PredecessorMemento predecessor in fact.Predecessors)
                    AddFactTree(messageBody, predecessor.ID);
                messageBody.Add(new IdentifiedFactMemento(factId, fact));
            }
        }

        private void SendMessageBodiesToServer(IEnumerable<MessageBodyMemento> messageBodies)
        {
            foreach (MessageBodyMemento messageBody in messageBodies)
            {
                CorrespondenceFact pivot = _repository.GetFactByID(messageBody.PivotId);
                foreach (ClientEndpoint postEndpoint in _postEndpoints)
                {
                    if (postEndpoint.IsCompatibleWith(pivot))
                    {
                        string url = postEndpoint.GetUrl(pivot);
                        _network.SendToServer(url, messageBody);
                    }
                }
            }
        }
    }
}
