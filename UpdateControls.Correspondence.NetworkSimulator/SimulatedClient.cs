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
        private List<ClientGetEndpoint> _getEndpoints = new List<ClientGetEndpoint>();

        public SimulatedClient(SimulatedNetwork network)
        {
            _network = network;

            network.AttachClient(this);
        }

        public SimulatedClient Get<FactType>(Func<IEnumerable<FactType>> pivots)
            where FactType : CorrespondenceFact
        {
            _getEndpoints.Add(new ClientGetEndpoint(
                typeof(FactType),
                () => pivots().OfType<CorrespondenceFact>()));
            return this;
        }

        public bool Synchronize()
        {
            bool any = false;
            if (SynchronizeOutgoing())
                any = true;
            if (SynchronizeIncoming())
                any = true;
            return any;
        }

        private bool SynchronizeOutgoing()
        {
            TimestampID timestamp = _repository.LoadOutgoingTimestamp(PROTOCOL_NAME, PEER_NAME);
            IEnumerable<FactTreeMemento> messageBodies = GetMessageBodies(ref timestamp);
            if (messageBodies.Any())
            {
                SendMessageBodiesToServer(messageBodies);
                _repository.SaveOutgoingTimestamp(PROTOCOL_NAME, PEER_NAME, timestamp);
                return true;
            }
            return false;
        }

        private bool SynchronizeIncoming()
        {
            bool any = false;
            foreach (ClientGetEndpoint getEndpoint in _getEndpoints)
            {
                foreach (CorrespondenceFact pivot in getEndpoint.Pivots)
                {
                    FactTreeMemento rootTree = new FactTreeMemento();
                    FactID rootId = _repository.IDOfFact(pivot);
                    AddToFactTree(rootTree, rootId);
                    TimestampID timestamp = _repository.LoadIncomingTimestamp(PROTOCOL_NAME, PEER_NAME, pivot);
                    FactTreeMemento messageBody = _network.GetFromServer(rootTree, rootId, timestamp);
                    if (messageBody.Facts.Any())
                    {
                        timestamp = ReceiveMessage(messageBody);
                        _repository.SaveIncomingTimestamp(PROTOCOL_NAME, PEER_NAME, pivot, timestamp);
                        any = true;
                    }
                }
            }
            return any;
        }

        private IEnumerable<FactTreeMemento> GetMessageBodies(ref TimestampID timestamp)
        {
            IDictionary<FactID, FactTreeMemento> messageBodiesByPivotId = new Dictionary<FactID, FactTreeMemento>();
            IEnumerable<MessageMemento> recentMessages = _repository.LoadRecentMessages(timestamp);
            foreach (MessageMemento message in recentMessages)
            {
                if (message.FactId.key > timestamp.key)
                    timestamp.key = message.FactId.key;
                FactTreeMemento messageBody;
                if (!messageBodiesByPivotId.TryGetValue(message.PivotId, out messageBody))
                {
                    messageBody = new FactTreeMemento();
                    messageBodiesByPivotId.Add(message.PivotId, messageBody);
                }
                AddToFactTree(messageBody, message.FactId);
            }
            return messageBodiesByPivotId.Values;
        }

        private void SendMessageBodiesToServer(IEnumerable<FactTreeMemento> messageBodies)
        {
            foreach (FactTreeMemento messageBody in messageBodies)
                _network.SendToServer(messageBody);
        }
    }
}
