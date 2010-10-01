using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System;
using UpdateControls.Correspondence.Async;
using System.Threading;

namespace UpdateControls.Correspondence
{
    class Network
    {
        private const long ClientDatabasId = 0;

        private Model _model;
        private IStorageStrategy _storageStrategy;
        private List<Subscription> _subscriptions = new List<Subscription>();
        private List<ICommunicationStrategy> _communicationStrategies = new List<ICommunicationStrategy>();
        private List<IAsynchronousCommunicationStrategy> _asynchronousCommunicationStrategies = new List<IAsynchronousCommunicationStrategy>();

        private Queue<SynchronizeResult> _synchronizeQueue = new Queue<SynchronizeResult>();

        internal class PushSubscriptionProxy : IDisposable
        {
            private Network _network;
            private IAsynchronousCommunicationStrategy _strategy;
            private CorrespondenceFact _pivot;
            private IPushSubscription _pushSubscription = null;

            public PushSubscriptionProxy(Network network, IAsynchronousCommunicationStrategy strategy, CorrespondenceFact pivot)
            {
                _network = network;
                _strategy = strategy;
                _pivot = pivot;
            }

            public void Subscribe()
            {
                if (_pushSubscription == null)
                {
 					FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabasId, 0L);
					FactID pivotId = _pivot.ID;
					_network.AddToFactTree(pivotTree, pivotId);
                    _pushSubscription = _strategy.SubscribeForPush(
                        pivotTree, 
                        pivotId, 
                        messageBody => _network.ReceiveMessage(messageBody));
                }
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                    return true;
                PushSubscriptionProxy that = obj as PushSubscriptionProxy;
                if (that == null)
                    return false;
                return this._strategy == that._strategy && this._pivot == that._pivot;
            }

            public override int GetHashCode()
            {
                return _strategy.GetHashCode() * 37 + _pivot.GetHashCode();
            }

            public void Dispose()
            {
                if (_pushSubscription != null)
                    _pushSubscription.Unsubscribe();
                _pushSubscription = null;
            }
        }

        private List<PushSubscriptionProxy> _pushSubscriptions = new List<PushSubscriptionProxy>();
        private Dependent _depPushSubscriptions;

        public Network(Model model, IStorageStrategy storageStrategy)
        {
            _model = model;
            _storageStrategy = storageStrategy;

            _depPushSubscriptions = new Dependent(UpdatePushSubscriptions);
            _depPushSubscriptions.Invalidated += TriggerAsync;
        }

        public void Subscribe(Subscription subscription)
        {
            _subscriptions.Add(subscription);
        }

        public void AddCommunicationStrategy(ICommunicationStrategy communicationStrategy)
        {
            _communicationStrategies.Add(communicationStrategy);
        }

        public void AddAsynchronousCommunicationStrategy(IAsynchronousCommunicationStrategy asynchronousCommunicationStrategy)
        {
            _asynchronousCommunicationStrategies.Add(asynchronousCommunicationStrategy);
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
                        if (pivot == null)
                            continue;

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

        public void BeginSynchronize(AsyncCallback callback, object state)
        {
            if (!_asynchronousCommunicationStrategies.Any())
                throw new CorrespondenceException("Register at least one asynchronous communication strategy before calling BeginSynchronize.");

            SynchronizeResult result = new SynchronizeResult(
                delegate(IAsyncResult r)
                {
                    ServeNextSynchronize(r);
                    callback(r);
                }, state);
            bool firstInLine = false;
            lock (this)
            {
                firstInLine = !_synchronizeQueue.Any();
                _synchronizeQueue.Enqueue(result);
            }
            if (firstInLine)
            {
                BeginSynchronizeOutgoing(result);
                BeginSynchronizeIncoming(result);
            }
        }

		public bool EndSynchronize(IAsyncResult result)
		{
			SynchronizeResult r = (SynchronizeResult)result;
			return (r.IncomingResult ?? false) || (r.OutgoingResult ?? false);
		}

        private void ServeNextSynchronize(IAsyncResult expected)
        {
            lock (this)
            {
                SynchronizeResult actual = _synchronizeQueue.Dequeue();
                if (actual != expected)
                    throw new CorrespondenceException("The synchronization queue is corrupt.");
                if (_synchronizeQueue.Any())
                {
                    SynchronizeResult result = _synchronizeQueue.Peek();
                    _depPushSubscriptions.OnGet();
                    BeginSynchronizeOutgoing(result);
                    BeginSynchronizeIncoming(result);
                }
            }
        }

        private void BeginSynchronizeOutgoing(SynchronizeResult result)
		{
			bool any = false;
			ResultAggregate communicationStrategyAggregate = new ResultAggregate(delegate()
			{
				result.OutgoingFinished(any);
			});
			foreach (IAsynchronousCommunicationStrategy asynchronousCommunicationStrategy in _asynchronousCommunicationStrategies)
			{
				string protocolName = asynchronousCommunicationStrategy.ProtocolName;
				string peerName = asynchronousCommunicationStrategy.PeerName;

				TimestampID timestamp = _storageStrategy.LoadOutgoingTimestamp(protocolName, peerName);
				IEnumerable<FactTreeMemento> messageBodies = GetMessageBodies(ref timestamp);
				if (messageBodies.Any())
				{
					any = true;
					communicationStrategyAggregate.Begin();
					ResultAggregate messageBodyAggregate = new ResultAggregate(delegate()
					{
						_storageStrategy.SaveOutgoingTimestamp(protocolName, peerName, timestamp);
						communicationStrategyAggregate.End();
					});
					foreach (FactTreeMemento messageBody in messageBodies)
					{
						messageBodyAggregate.Begin();
						asynchronousCommunicationStrategy.BeginPost(messageBody, messageBodyAggregate.End);
					}
					messageBodyAggregate.Close();
				}
			}

			communicationStrategyAggregate.Close();
		}

        private void BeginSynchronizeIncoming(SynchronizeResult result)
		{
			bool any = false;
			ResultAggregate communicationStragetyAggregate = new ResultAggregate(delegate()
			{
				result.IncomingFinished(any);
			});
			foreach (IAsynchronousCommunicationStrategy asynchronousCommunicationStrategy in _asynchronousCommunicationStrategies)
			{
				string protocolName = asynchronousCommunicationStrategy.ProtocolName;
				string peerName = asynchronousCommunicationStrategy.PeerName;

				foreach (Subscription subscription in _subscriptions)
				{
					foreach (CorrespondenceFact pivot in subscription.Pivots)
					{
                        if (pivot == null)
                            continue;

						FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabasId, 0L);
						FactID pivotId = pivot.ID;
						AddToFactTree(pivotTree, pivotId);
						TimestampID timestamp = _storageStrategy.LoadIncomingTimestamp(protocolName, peerName, pivotId);
						communicationStragetyAggregate.Begin();
						asynchronousCommunicationStrategy.BeginGet(pivotTree, pivotId, timestamp, delegate(FactTreeMemento messageBody)
						{
							if (messageBody.Facts.Any())
							{
								any = true;
								TimestampID newTimestamp = ReceiveMessage(messageBody);
								_storageStrategy.SaveIncomingTimestamp(protocolName, peerName, pivotId, newTimestamp);
							}
							communicationStragetyAggregate.End();
						});
					}
				}
			}

			communicationStragetyAggregate.Close();
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

        private void UpdatePushSubscriptions()
        {
            using (var bin = _pushSubscriptions.Recycle())
            {
                var pivots = _subscriptions
                    .SelectMany(subscription => subscription.Pivots)
                    .Where(pivot => pivot != null);
                var pushSubscriptions =
                    from strategy in _asynchronousCommunicationStrategies
                    from pivot in pivots
                    select bin.Extract(new PushSubscriptionProxy(this, strategy, pivot));

                _pushSubscriptions = pushSubscriptions.ToList();

                foreach (PushSubscriptionProxy pushSubscription in _pushSubscriptions)
                {
                    pushSubscription.Subscribe();
                }
            }
        }

        private void TriggerAsync()
        {
            BeginSynchronize(a =>
            {
                if (EndSynchronize(a))
                    TriggerAsync();
            }, null);
        }
    }
}
