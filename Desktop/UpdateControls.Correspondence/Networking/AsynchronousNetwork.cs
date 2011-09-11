using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Fields;

namespace UpdateControls.Correspondence.Networking
{
	partial class AsynchronousNetwork
	{
		private const long ClientDatabaseId = 0;

		private ISubscriptionProvider _subscriptionProvider;
		private Model _model;
		private IStorageStrategy _storageStrategy;

        private List<AsynchronousServerProxy> _serverProxies = new List<AsynchronousServerProxy>();
        private Independent<bool> _sending = new Independent<bool>();
        private Independent<bool> _receiving = new Independent<bool>();
        private Independent<Exception> _lastException = new Independent<Exception>();

		private List<PushSubscriptionProxy> _pushSubscriptions = new List<PushSubscriptionProxy>();
		private Dependent _depPushSubscriptions;

		public AsynchronousNetwork(ISubscriptionProvider subscriptionProvider, Model model, IStorageStrategy storageStrategy)
		{
			_subscriptionProvider = subscriptionProvider;
			_model = model;
			_storageStrategy = storageStrategy;

			_depPushSubscriptions = new Dependent(UpdatePushSubscriptions);
			_depPushSubscriptions.Invalidated += TriggerSubscriptionUpdate;
		}

		public void AddAsynchronousCommunicationStrategy(IAsynchronousCommunicationStrategy asynchronousCommunicationStrategy)
		{
            int peerId = _storageStrategy.SavePeer(
                asynchronousCommunicationStrategy.ProtocolName,
                asynchronousCommunicationStrategy.PeerName);
            asynchronousCommunicationStrategy.MessageReceived += messageBody =>
            {
                _model.ReceiveMessage(messageBody, peerId);
                // Trigger a receive on normal channels. This updates the
                // timestamp and pulls down any messages that were too long
                // for the push buffer.
                BeginReceiving();
            };
            _serverProxies.Add(new AsynchronousServerProxy
            {
                CommunicationStrategy = asynchronousCommunicationStrategy,
                PeerId = peerId
            });
		}

        public bool Synchronizing
        {
            get
            {
                lock (this)
                {
                    return _receiving || _sending;
                }
            }
        }

        public Exception LastException
        {
            get
            {
                lock (this)
                {
                    return _lastException;
                }
            }
        }

        public void BeginSending()
        {
            lock (this)
            {
                if (_sending)
                    return;
            }
            Send();
        }

        private void Send()
		{
            bool sending = false;
			foreach (AsynchronousServerProxy serverProxy in _serverProxies)
			{
				TimestampID timestamp = _storageStrategy.LoadOutgoingTimestamp(serverProxy.PeerId);
                FactTreeMemento messageBodies = _model.GetMessageBodies(ref timestamp, serverProxy.PeerId, new List<UnpublishMemento>());
                if (messageBodies != null && messageBodies.Facts.Any())
                {
                    sending = true;
                    serverProxy.CommunicationStrategy.BeginPost(messageBodies, _model.ClientDatabaseGuid, delegate(bool succeeded)
                    {
                        if (succeeded)
                        {
                            _storageStrategy.SaveOutgoingTimestamp(serverProxy.PeerId, timestamp);
                            Send();
                        }
                        else
                        {
                            OnSendError(null);
                        }
                    }, OnSendError);
                }
			}

            lock (this)
            {
                _sending.Value = sending;
            }
		}

        public void BeginReceiving()
        {
            lock (this)
            {
                if (_receiving)
                    return;
            }

            Receive();
        }

        public void Receive()
        {
            FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabaseId);
            List<FactID> pivotIds = new List<FactID>();
            lock (this)
            {
                _depPushSubscriptions.OnGet();
                GetPivots(pivotTree, pivotIds);
            }

            bool anyPivots = pivotIds.Any();
            if (anyPivots)
            {
                foreach (AsynchronousServerProxy serverProxy in _serverProxies)
                {
                    List<PivotMemento> pivots = new List<PivotMemento>();
                    foreach (FactID pivotId in pivotIds)
                    {
                        TimestampID timestamp = _storageStrategy.LoadIncomingTimestamp(serverProxy.PeerId, pivotId);
                        pivots.Add(new PivotMemento(pivotId, timestamp));
                    }
                    serverProxy.CommunicationStrategy.BeginGetMany(pivotTree, pivots, _model.ClientDatabaseGuid, delegate(FactTreeMemento messageBody, IEnumerable<PivotMemento> newTimestamps)
                    {
                        bool receivedFacts = messageBody.Facts.Any();
                        if (receivedFacts)
                        {
                            _model.ReceiveMessage(messageBody, serverProxy.PeerId);
                            foreach (PivotMemento pivot in newTimestamps)
                            {
                                _storageStrategy.SaveIncomingTimestamp(serverProxy.PeerId, pivot.PivotId, pivot.Timestamp);
                            }
                        }
                        if (receivedFacts || serverProxy.CommunicationStrategy.IsLongPolling)
                            Receive();
                        else
                        {
                            lock (this)
                            {
                                _receiving.Value = false;
                            }
                        }
                    }, OnReceiveError);
                }
            }

            lock (this)
            {
                _receiving.Value = anyPivots;
            }
        }

		private void UpdatePushSubscriptions()
		{
			using (var bin = _pushSubscriptions.Recycle())
			{
				var pivots = _subscriptionProvider.Subscriptions
					.SelectMany(subscription => subscription.Pivots)
					.Where(pivot => pivot != null);
				var pushSubscriptions =
					from serverProxy in _serverProxies
					from pivot in pivots
					select bin.Extract(new PushSubscriptionProxy(_model, serverProxy.CommunicationStrategy, pivot));

				_pushSubscriptions = pushSubscriptions.ToList();

				foreach (PushSubscriptionProxy pushSubscription in _pushSubscriptions)
				{
                    pushSubscription.Subscribe();
				}
			}
		}

        private void GetPivots(FactTreeMemento pivotTree, List<FactID> pivotIds)
        {
            foreach (Subscription subscription in _subscriptionProvider.Subscriptions)
            {
                foreach (CorrespondenceFact pivot in subscription.Pivots)
                {
                    if (pivot == null)
                        continue;

                    FactID pivotId = pivot.ID;
                    _model.AddToFactTree(pivotTree, pivotId);
                    pivotIds.Add(pivotId);
                }
            }
        }

        private void OnSendError(Exception exception)
        {
            lock (this)
            {
                _sending.Value = false;
                _lastException.Value = exception;
            }
        }

        private void OnReceiveError(Exception exception)
        {
            lock (this)
            {
                _receiving.Value = false;
                _lastException.Value = exception;
            }
        }

        private void TriggerSubscriptionUpdate()
        {
            RunOnUIThread(new Action(() =>
            {
                lock (this)
                {
                    _depPushSubscriptions.OnGet();
                }
            }));
        }
    }
}
