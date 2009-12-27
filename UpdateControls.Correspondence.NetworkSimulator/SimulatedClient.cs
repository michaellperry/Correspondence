using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class SimulatedClient : SimulatedMachine
    {
        private const string PROTOCOL_NAME = "simulator";
        private const string PEER_NAME = "client";

        private SimulatedNetwork _network;
        private List<ClientPostEndpoint> _postEndpoints = new List<ClientPostEndpoint>();
        private List<ClientGetEndpoint> _getEndpoints = new List<ClientGetEndpoint>();

        public SimulatedClient(SimulatedNetwork network)
        {
            _network = network;

            network.AttachClient(this);
        }

        public SimulatedClient Post<FactType>(Func<FactType, string> factToPath)
            where FactType : CorrespondenceFact
        {
            _postEndpoints.Add(new ClientPostEndpoint(
                typeof(FactType),
                fact => factToPath((FactType)fact)));
            return this;
        }

        public SimulatedClient Get<FactType>(Func<IEnumerable<FactType>> pivots, Func<FactType, string> factToPath)
            where FactType : CorrespondenceFact
        {
            _getEndpoints.Add(new ClientGetEndpoint(
                typeof(FactType),
                () => pivots().OfType<CorrespondenceFact>(),
                fact => factToPath((FactType)fact)));
            return this;
        }

        public void Synchronize()
        {
            SynchronizeOutgoing();
            SynchronizeIncoming();
        }

        private void SynchronizeOutgoing()
        {
            TimestampID timestamp = _repository.LoadOutgoingTimestamp(PROTOCOL_NAME, PEER_NAME);
            IEnumerable<MessageBodyMemento> messageBodies = GetMessageBodies(ref timestamp);
            SendMessageBodiesToServer(messageBodies);
            _repository.SaveOutgoingTimestamp(PROTOCOL_NAME, PEER_NAME, timestamp);
        }

        private void SynchronizeIncoming()
        {
            foreach (ClientGetEndpoint getEndpoint in _getEndpoints)
            {
                foreach (CorrespondenceFact pivot in getEndpoint.Pivots)
                {
                    string path = getEndpoint.GetPath(pivot);
                    TimestampID timestamp = _repository.LoadIncomingTimestamp(PROTOCOL_NAME, PEER_NAME);
                    MessageBodyMemento messageBody = _network.GetFromServer(path, timestamp);
                    if (messageBody.Facts.Any())
                    {
                        timestamp = ReceiveMessage(messageBody, pivot);
                        _repository.SaveIncomingTimestamp(PROTOCOL_NAME, PEER_NAME, timestamp);
                    }
                }
            }
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

        private void SendMessageBodiesToServer(IEnumerable<MessageBodyMemento> messageBodies)
        {
            foreach (MessageBodyMemento messageBody in messageBodies)
            {
                CorrespondenceFact pivot = _repository.GetFactByID(messageBody.PivotId);
                foreach (ClientEndpoint postEndpoint in _postEndpoints)
                {
                    if (postEndpoint.IsCompatibleWith(pivot))
                    {
                        string url = postEndpoint.GetPath(pivot);
                        _network.SendToServer(url, messageBody);
                    }
                }
            }
        }
    }
}
