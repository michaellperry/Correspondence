using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Fields;

namespace UpdateControls.Correspondence.Networking
{
	internal class AsynchronousServerProxy
	{
        private enum ReceiveState
        {
            NotReceiving,
            Receiving,
            Invalidated
        }

		private const long ClientDatabaseId = 0;

        private IAsynchronousCommunicationStrategy _communicationStrategy;
        private int _peerId;

        private ISubscriptionProvider _subscriptionProvider;
		private Model _model;
		private IStorageStrategy _storageStrategy;

        private Independent<bool> _sending = new Independent<bool>();
        private Independent<ReceiveState> _receiveState = new Independent<ReceiveState>();
        private Independent<Exception> _lastException = new Independent<Exception>();

		private List<PushSubscriptionProxy> _pushSubscriptions = new List<PushSubscriptionProxy>();
		private Dependent _depPushSubscriptions;

        public AsynchronousServerProxy(
            ISubscriptionProvider subscriptionProvider, 
            Model model, 
            IStorageStrategy storageStrategy,
            IAsynchronousCommunicationStrategy communicationStrategy, 
            int peerId)
		{
			_subscriptionProvider = subscriptionProvider;
			_model = model;
			_storageStrategy = storageStrategy;
            _communicationStrategy = communicationStrategy;
            _peerId = peerId;

			_depPushSubscriptions = new Dependent(UpdatePushSubscriptions);
		}

        public IAsynchronousCommunicationStrategy CommunicationStrategy
        {
            get { return _communicationStrategy; }
        }

        public int PeerId
        {
            get { return _peerId; }
        }

        public bool Synchronizing
        {
            get
            {
                lock (this)
                {
                    return
                        _receiveState.Value == ReceiveState.Receiving ||
                        _receiveState.Value == ReceiveState.Invalidated ||
                        _sending.Value == true;
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
            lock (this)
            {
                TimestampID timestamp = _storageStrategy.LoadOutgoingTimestamp(_peerId);
                List<UnpublishMemento> unpublishedMessages = new List<UnpublishMemento>();
                FactTreeMemento messageBodies = _model.GetMessageBodies(ref timestamp, _peerId, unpublishedMessages);
                bool anyMessages = messageBodies != null && messageBodies.Facts.Any();
                if (anyMessages)
                {
                    _communicationStrategy.BeginPost(messageBodies, _model.ClientDatabaseGuid, unpublishedMessages, delegate(bool succeeded)
                    {
                        if (succeeded)
                        {
                            _storageStrategy.SaveOutgoingTimestamp(_peerId, timestamp);
                            Send();
                        }
                        else
                        {
                            OnSendError(null);
                        }
                    }, OnSendError);
                }

                _sending.Value = anyMessages;
            }
        }

        public void BeginReceiving()
        {
            if (ShouldReceive())
                Receive();
        }

        private bool ShouldReceive()
        {
            lock (this)
            {
                if (_receiveState.Value == ReceiveState.NotReceiving)
                {
                    _receiveState.Value = ReceiveState.Receiving;
                    return true;
                }
                else if (_receiveState.Value == ReceiveState.Receiving)
                {
                    _receiveState.Value = ReceiveState.Invalidated;
                    return false;
                }
                else //if (_receiveState.Value == ReceiveState.Invalidated)
                {
                    return false;
                }
            }
        }

        private void Receive()
        {
            lock (this)
            {
                FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabaseId);
                List<FactID> pivotIds = new List<FactID>();
                _depPushSubscriptions.OnGet();
                GetPivots(pivotTree, pivotIds, _peerId);

                bool anyPivots = pivotIds.Any();
                if (anyPivots)
                {
                    List<PivotMemento> pivots = new List<PivotMemento>();
                    foreach (FactID pivotId in pivotIds)
                    {
                        TimestampID timestamp = _storageStrategy.LoadIncomingTimestamp(_peerId, pivotId);
                        pivots.Add(new PivotMemento(pivotId, timestamp));
                    }
                    _communicationStrategy.BeginGetMany(pivotTree, pivots, _model.ClientDatabaseGuid, delegate(FactTreeMemento messageBody, IEnumerable<PivotMemento> newTimestamps)
                    {
                        bool receivedFacts = messageBody.Facts.Any();
                        if (receivedFacts)
                        {
                            _model.ReceiveMessage(messageBody, _peerId);
                            foreach (PivotMemento pivot in newTimestamps)
                            {
                                _storageStrategy.SaveIncomingTimestamp(_peerId, pivot.PivotId, pivot.Timestamp);
                            }
                        }
                        if (receivedFacts || _communicationStrategy.IsLongPolling)
                            Receive();
                        else
                        {
                            if (EndReceiving())
                                Receive();
                        }
                    }, OnReceiveError);
                }

                if (_receiveState.Value == ReceiveState.Receiving && !anyPivots)
                    _receiveState.Value = ReceiveState.NotReceiving;
            }
        }

        private bool EndReceiving()
        {
            lock (this)
            {
                if (_receiveState.Value == ReceiveState.Invalidated)
                {
                    _receiveState.Value = ReceiveState.Receiving;
                    return true;
                }
                else
                {
                    _receiveState.Value = ReceiveState.NotReceiving;
                    return false;
                }
            }
        }

        public void Notify(CorrespondenceFact pivot, string text1, string text2)
        {
            FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabaseId);
            _model.AddToFactTree(pivotTree, pivot.ID, _peerId);
            _communicationStrategy.Notify(
                pivotTree,
                pivot.ID,
                _model.ClientDatabaseGuid,
                text1,
                text2);
        }

        private void UpdatePushSubscriptions()
		{
			using (var bin = _pushSubscriptions.Recycle())
			{
				var pivots = _subscriptionProvider.Subscriptions
					.SelectMany(subscription => subscription.Pivots)
					.Where(pivot => pivot != null);
				var pushSubscriptions =
					from pivot in pivots
					select bin.Extract(new PushSubscriptionProxy(_model, this, pivot));

				_pushSubscriptions = pushSubscriptions.ToList();

				foreach (PushSubscriptionProxy pushSubscription in _pushSubscriptions)
				{
                    pushSubscription.Subscribe();
				}
			}
		}

        private void GetPivots(FactTreeMemento pivotTree, List<FactID> pivotIds, int peerId)
        {
            foreach (Subscription subscription in _subscriptionProvider.Subscriptions)
            {
                if (subscription.Pivots != null)
                {
                    foreach (CorrespondenceFact pivot in subscription.Pivots)
                    {
                        if (pivot == null)
                            continue;

                        FactID pivotId = pivot.ID;
                        _model.AddToFactTree(pivotTree, pivotId, peerId);
                        pivotIds.Add(pivotId);
                    }
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
                _receiveState.Value = ReceiveState.NotReceiving;
                _lastException.Value = exception;
            }
        }

        public void TriggerSubscriptionUpdate()
        {
            lock (this)
            {
                _depPushSubscriptions.OnGet();
            }
        }

        public void AfterTriggerSubscriptionUpdate()
        {
            if (_communicationStrategy.IsLongPolling)
                _communicationStrategy.Interrupt(_model.ClientDatabaseGuid);

            BeginReceiving();
        }
	}
}
