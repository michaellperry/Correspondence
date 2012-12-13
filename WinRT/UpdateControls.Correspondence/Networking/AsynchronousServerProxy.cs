using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Fields;

namespace UpdateControls.Correspondence.Networking
{
	internal class AsynchronousServerProxy
	{
        private enum SendState
        {
            Idle,
            Sending,
            SendRequested
        }

        private enum ReceiveState
        {
            Idle,
            Receiving,
            ReceiveRequested,
            Polling
        }

		private const long ClientDatabaseId = 0;

        private readonly IAsynchronousCommunicationStrategy _communicationStrategy;
        private readonly int _peerId;

        private readonly ISubscriptionProvider _subscriptionProvider;
        private readonly Model _model;
        private readonly IStorageStrategy _storageStrategy;

        private Independent<SendState> _sendState = new Independent<SendState>();
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
                        _receiveState.Value == ReceiveState.ReceiveRequested ||
                        _sendState.Value == SendState.Sending ||
                        _sendState.Value == SendState.SendRequested;
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
                if (_sendState.Value == SendState.Idle)
                    _sendState.Value = SendState.Sending;
                else
                {
                    _sendState.Value = SendState.SendRequested;
                    return;
                }
            }
            Send();
        }

        private async void Send()
        {
            try
            {
                TimestampID timestamp = await _storageStrategy.LoadOutgoingTimestampAsync(_peerId);
                MessagesToSend messagesToSend;
                while ((messagesToSend = await GetMessagesToSendAsync(timestamp)) != null)
                {
                    await _communicationStrategy.PostAsync(
                        messagesToSend.MessageBodies,
                        await _model.GetClientDatabaseGuidAsync(),
                        messagesToSend.UnpublishedMessages);
                    timestamp = messagesToSend.Timestamp;
                    await _storageStrategy.SaveOutgoingTimestampAsync(_peerId, timestamp);
                }
            }
            catch (Exception e)
            {
                OnSendError(e);
            }
        }

        private async Task<MessagesToSend> GetMessagesToSendAsync(TimestampID timestamp)
        {
            while (true)
            {
                // I'm about to check for messages, so if you've requested
                // a send, you'll be taken care of.
                lock (this)
                {
                    if (_sendState.Value == SendState.SendRequested)
                        _sendState.Value = SendState.Sending;
                }

                // Check for messages.
                List<UnpublishMemento> unpublishedMessages = new List<UnpublishMemento>();
                GetResultMemento result = await _model.GetMessageBodiesAsync(timestamp, _peerId, unpublishedMessages);
                FactTreeMemento messageBodies = result.FactTree;

                // If we have any, post them.
                bool anyMessages = messageBodies != null && messageBodies.Facts.Any();
                if (anyMessages)
                    return new MessagesToSend
                    {
                        Timestamp = result.Timestamp,
                        MessageBodies = messageBodies,
                        UnpublishedMessages = unpublishedMessages
                    };

                // If not, go idle unless another send was requested.
                lock (this)
                {
                    if (_sendState.Value == SendState.Sending)
                    {
                        _sendState.Value = SendState.Idle;
                        return null;
                    }
                }
            }
        }

        public void BeginReceiving()
        {
            lock (this)
            {
                if (_receiveState.Value == ReceiveState.Idle)
                    _receiveState.Value = ReceiveState.Receiving;
                else if (_receiveState.Value == ReceiveState.Receiving)
                {
                    if (!_communicationStrategy.IsLongPolling)
                        _receiveState.Value = ReceiveState.ReceiveRequested;
                    return;
                }
            }

            Receive();
        }

        private async void Receive()
        {
            try
            {
                GetManyResultMemento messagesToReceive;
                while ((messagesToReceive = await GetMessagesToReceive()) != null)
                {
                    _model.ReceiveMessage(messagesToReceive.MessageBody, _peerId);
                    foreach (PivotMemento pivot in messagesToReceive.NewTimestamps)
                    {
                        await _storageStrategy.SaveIncomingTimestampAsync(_peerId, pivot.PivotId, pivot.Timestamp);
                    }
                }
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }
        }

        private async Task<GetManyResultMemento> GetMessagesToReceive()
        {
            while (true)
            {
                // I'm about to receive messages, so if you've requested
                // a receive, you'll be taken care of.
                lock (this)
                {
                    if (_receiveState.Value == ReceiveState.ReceiveRequested)
                        _receiveState.Value = ReceiveState.Receiving;
                }

                // Get the subscriptions
                FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabaseId);
                List<FactID> pivotIds = new List<FactID>();
                lock (this)
                {
                    _depPushSubscriptions.OnGet();
                }
                await GetPivotsAsync(pivotTree, pivotIds, _peerId);

                bool anyPivots = pivotIds.Any();
                if (anyPivots)
                {
                    // Get the incoming timestamps.
                    List<PivotMemento> pivots = new List<PivotMemento>();
                    foreach (FactID pivotId in pivotIds)
                    {
                        TimestampID timestamp = await _storageStrategy.LoadIncomingTimestampAsync(_peerId, pivotId);
                        pivots.Add(new PivotMemento(pivotId, timestamp));
                    }

                    // Get the messages from the server.
                    GetManyResultMemento result = await _communicationStrategy.GetManyAsync(
                        pivotTree,
                        pivots,
                        await _model.GetClientDatabaseGuidAsync());

                    bool receivedFacts = result.MessageBody.Facts.Any();
                    if (receivedFacts)
                        return result;

                    // No messages yet, but keep polling.
                    if (_communicationStrategy.IsLongPolling)
                        continue;
                }

                // Nothing received: go idle unless another receive was requested.
                lock (this)
                {
                    if (_receiveState.Value == ReceiveState.Receiving)
                    {
                        _receiveState.Value = ReceiveState.Idle;
                        return null;
                    }
                }
            }
        }

        public async void Notify(CorrespondenceFact pivot, string text1, string text2)
        {
            FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabaseId);
            await _model.AddToFactTreeAsync(pivotTree, pivot.ID, _peerId);
            await _communicationStrategy.NotifyAsync(
                pivotTree,
                pivot.ID,
                await _model.GetClientDatabaseGuidAsync(),
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

        private async Task GetPivotsAsync(FactTreeMemento pivotTree, List<FactID> pivotIds, int peerId)
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
                        await _model.AddToFactTreeAsync(pivotTree, pivotId, peerId);
                        pivotIds.Add(pivotId);
                    }
                }
            }
        }

        private void OnSendError(Exception exception)
        {
            lock (this)
            {
                _sendState.Value = SendState.Idle;
                _lastException.Value = exception;
            }
        }

        private void OnReceiveError(Exception exception)
        {
            lock (this)
            {
                _receiveState.Value = ReceiveState.Idle;
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

        public async void AfterTriggerSubscriptionUpdate()
        {
            if (_communicationStrategy.IsLongPolling)
                await _communicationStrategy.InterruptAsync(
                    await _model.GetClientDatabaseGuidAsync());

            BeginReceiving();
        }
	}
}
