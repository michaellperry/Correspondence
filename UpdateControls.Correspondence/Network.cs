using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System;

namespace UpdateControls.Correspondence
{
    class Network
    {
        private const long ClientDatabasId = 0;

        private Model _model;
        private IStorageStrategy _storageStrategy;
        private List<Subscription> _subscriptions = new List<Subscription>();
        private List<ICommunicationStrategy> _communicationStrategies = new List<ICommunicationStrategy>();

        public Network(Model model, IStorageStrategy storageStrategy)
        {
            _model = model;
            _storageStrategy = storageStrategy;
        }

        public void Subscribe(Subscription subscription)
        {
            _subscriptions.Add(subscription);
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

                foreach (Subscription subscription in _subscriptions)
                {
                    foreach (CorrespondenceFact pivot in subscription.Pivots)
                    {
                        FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabasId, 0L);
                        FactID pivotId = pivot.ID;
                        AddToFactTree(pivotTree, pivotId);
                        TimestampID timestamp = _storageStrategy.LoadIncomingTimestamp(protocolName, peerName, pivotId);
                        FactTreeMemento messageBody = communicationStrategy.Get(pivotTree, pivotId, timestamp);
                        if (messageBody.Facts.Any())
                        {
                            timestamp = ReceiveMessage(messageBody);
                            _storageStrategy.SaveIncomingTimestamp(protocolName, peerName, pivotId, timestamp);
                            any = true;
                        }
                    }
                }
            }

            return any;
        }

        class SynchronizeResult : IAsyncResult
        {
            private AsyncCallback _callback;
            private object _state;
            private bool _outgoingFinished = false;
            private bool _incomingFinished = false;

            public SynchronizeResult(AsyncCallback callback, object state)
            {
                _callback = callback;
                _state = state;
            }

            public void OutgoingFinished()
            {
                _outgoingFinished = true;
                Finish();
            }

            public void IncomingFinished()
            {
                _incomingFinished = true;
                Finish();
            }

            private void Finish()
            {
                if (_outgoingFinished && _incomingFinished)
                {
                    _callback(this);
                }
            }

            #region IAsyncResult Members

            public object AsyncState
            {
                get { throw new NotImplementedException(); }
            }

            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get { throw new NotImplementedException(); }
            }

            public bool CompletedSynchronously
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsCompleted
            {
                get { throw new NotImplementedException(); }
            }

            #endregion
        }

        public void BeginSynchronize(AsyncCallback callback, object state)
        {
            SynchronizeResult result = new SynchronizeResult(callback, state);
            BeginSynchronizeOutgoing(result);
            BeginSynchronizeIncoming(result);
        }

        private void BeginSynchronizeOutgoing(SynchronizeResult result)
        {
            throw new NotImplementedException();
        }

        private void BeginSynchronizeIncoming(SynchronizeResult result)
        {
            throw new NotImplementedException();
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
                    messageBody = new FactTreeMemento(ClientDatabasId, 0L);
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
            foreach (IdentifiedFactMemento identifiedFact in messageBody.Facts)
            {
                FactMemento translatedMemento = new FactMemento(identifiedFact.Memento.FactType);
                translatedMemento.Data = identifiedFact.Memento.Data;
                translatedMemento.AddPredecessors(identifiedFact.Memento.Predecessors
                    .Select(remote => new PredecessorMemento(remote.Role, localIdByRemoteId[remote.ID])));
                FactID localId = _model.SaveFact(translatedMemento);
                FactID remoteId = identifiedFact.Id;
                localIdByRemoteId.Add(remoteId, localId);
            }

            return new TimestampID(messageBody.DatabaseId, messageBody.Timestamp);
        }
    }
}
