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
        private SimulatedNetwork _network;
        private IMessageRepository _repository;

        private const string PROTOCOL_NAME = "simulator";
        private const string PEER_NAME = "client";

        public SimulatedClient(SimulatedNetwork network)
        {
            _network = network;

            network.AttachClient(this);
        }

        public SimulatedClient Post<FactType>(Func<FactType, string> factToUrl)
            where FactType : CorrespondenceFact
        {
            return this;
        }

        public void AttachMessageRepository(IMessageRepository repository)
        {
            _repository = repository;
        }

        public void Synchronize()
        {
            TimestampID timestamp = _repository.LoadTimestamp(PROTOCOL_NAME, PEER_NAME);
            IDictionary<FactID, MessageBodyMemento> messages = new Dictionary<FactID,MessageBodyMemento>();
            IEnumerable<MessageMemento> recentMessages = _repository.LoadRecentMessages(ref timestamp);
            foreach (MessageMemento recentMessage in recentMessages)
            {
                MessageBodyMemento messageBody;
                if (!messages.TryGetValue(recentMessage.PivotId, out messageBody))
                {
                    messageBody = new MessageBodyMemento(recentMessage.PivotId);
                    messages.Add(recentMessage.PivotId, messageBody);
                }
                if (messageBody.Add(recentMessage.FactId, _repository.LoadFact(recentMessage.FactId)))
                {

                }
            }
            _repository.SaveTimestamp(PROTOCOL_NAME, PEER_NAME, timestamp);
        }
    }
}
