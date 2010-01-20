using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence
{
    class Network
    {
        private const long ClientDatabasId = 0;

        private Model _model;
        private IStorageStrategy _storageStrategy;
        private List<Interest> _interests = new List<Interest>();
        private List<ICommunicationStrategy> _communicationStrategies = new List<ICommunicationStrategy>();

        public Network(Model model, IStorageStrategy storageStrategy)
        {
            _model = model;
            _storageStrategy = storageStrategy;
        }

        public void AddInterest(Interest interest)
        {
            _interests.Add(interest);
        }

        public void AddCommunicationStrategy(ICommunicationStrategy communicationStrategy)
        {
            _communicationStrategies.Add(communicationStrategy);
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
            bool any = false;
            foreach (ICommunicationStrategy communicationStrategy in _communicationStrategies)
            {
                string protocolName = communicationStrategy.ProtocolName;
                string peerName = communicationStrategy.PeerName;

                TimestampID timestamp = _storageStrategy.LoadOutgoingTimestamp(protocolName, peerName);
                IEnumerable<FactTreeMemento> messageBodies = GetMessageBodies(ref timestamp);
                if (messageBodies.Any())
                {
                    foreach (FactTreeMemento messageBody in messageBodies)
                        communicationStrategy.Post(messageBody);
                    _storageStrategy.SaveOutgoingTimestamp(protocolName, peerName, timestamp);
                    any = true;
                }
            }

            return any;
        }

        private bool SynchronizeIncoming()
        {
            bool any = false;
            foreach (ICommunicationStrategy communicationStrategy in _communicationStrategies)
            {
                string protocolName = communicationStrategy.ProtocolName;
                string peerName = communicationStrategy.PeerName;

                foreach (Interest interest in _interests)
                {
                    foreach (CorrespondenceFact root in interest.Roots)
                    {
                        FactTreeMemento rootTree = new FactTreeMemento(ClientDatabasId);
                        FactID rootId = root.ID;
                        AddToFactTree(rootTree, rootId);
                        TimestampID timestamp = _storageStrategy.LoadIncomingTimestamp(protocolName, peerName, rootId);
                        FactTreeMemento messageBody = communicationStrategy.Get(rootTree, rootId, timestamp);
                        if (messageBody.Facts.Any())
                        {
                            timestamp = ReceiveMessage(messageBody);
                            _storageStrategy.SaveIncomingTimestamp(protocolName, peerName, rootId, timestamp);
                            any = true;
                        }
                    }
                }
            }

            return any;
        }

        private IEnumerable<FactTreeMemento> GetMessageBodies(ref TimestampID timestamp)
        {
            IDictionary<FactID, FactTreeMemento> messageBodiesByPivotId = new Dictionary<FactID, FactTreeMemento>();
            IEnumerable<MessageMemento> recentMessages = _storageStrategy.LoadRecentMessages(timestamp);
            foreach (MessageMemento message in recentMessages)
            {
                if (message.FactId.key > timestamp.Key)
                    timestamp = new TimestampID(ClientDatabasId, message.FactId.key);
                FactTreeMemento messageBody;
                if (!messageBodiesByPivotId.TryGetValue(message.PivotId, out messageBody))
                {
                    messageBody = new FactTreeMemento(ClientDatabasId);
                    messageBodiesByPivotId.Add(message.PivotId, messageBody);
                }
                AddToFactTree(messageBody, message.FactId);
            }
            return messageBodiesByPivotId.Values;
        }

        private void AddToFactTree(FactTreeMemento messageBody, FactID factId)
        {
            if (!messageBody.Contains(factId))
            {
                FactMemento fact = _model.LoadFact(factId);
                foreach (PredecessorMemento predecessor in fact.Predecessors)
                    AddToFactTree(messageBody, predecessor.ID);
                messageBody.Add(new IdentifiedFactMemento(factId, fact));
            }
        }

        private TimestampID ReceiveMessage(FactTreeMemento messageBody)
        {
            IDictionary<FactID, FactID> localIdByRemoteId = new Dictionary<FactID, FactID>();
            long maxRemoteId = 0;
            foreach (IdentifiedFactMemento identifiedFact in messageBody.Facts)
            {
                FactMemento translatedMemento = new FactMemento(identifiedFact.Memento.FactType);
                translatedMemento.Data = identifiedFact.Memento.Data;
                translatedMemento.AddPredecessors(identifiedFact.Memento.Predecessors
                    .Select(remote => new PredecessorMemento(remote.Role, localIdByRemoteId[remote.ID])));
                FactID localId = _model.SaveFact(translatedMemento);
                FactID remoteId = identifiedFact.Id;
                localIdByRemoteId.Add(remoteId, localId);

                if (remoteId.key > maxRemoteId)
                    maxRemoteId = remoteId.key;
            }
            return new TimestampID(messageBody.DatabaseId, maxRemoteId);
        }
    }
}
