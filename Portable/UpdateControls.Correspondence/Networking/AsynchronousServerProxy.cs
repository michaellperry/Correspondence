using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        private readonly ISubscriptionProvider _subscriptionProvider;
        private readonly Model _model;
        private readonly IStorageStrategy _storageStrategy;

        private int _peerId;

        private Independent<SendState> _sendState = new Independent<SendState>();
        private Independent<ReceiveState> _receiveState = new Independent<ReceiveState>();
        private Independent<Exception> _lastSendException = new Independent<Exception>();
        private Independent<Exception> _lastReceiveException = new Independent<Exception>();

		private List<PushSubscriptionProxy> _pushSubscriptions = new List<PushSubscriptionProxy>();
		private Dependent _depPushSubscriptions;

        public AsynchronousServerProxy(
            ISubscriptionProvider subscriptionProvider, 
            Model model, 
            IStorageStrategy storageStrategy,
            IAsynchronousCommunicationStrategy communicationStrategy)
		{
			_subscriptionProvider = subscriptionProvider;
			_model = model;
			_storageStrategy = storageStrategy;
            _communicationStrategy = communicationStrategy;

			_depPushSubscriptions = new Dependent(UpdatePushSubscriptions);
		}

        public IAsynchronousCommunicationStrategy CommunicationStrategy
        {
            get { return _communicationStrategy; }
        }

        public async Task<int> GetPeerId()
        {
            if (_peerId == 0)
            {
                _peerId = await _storageStrategy.SavePeerAsync(
                    _communicationStrategy.ProtocolName,
                    _communicationStrategy.PeerName);
            }
            return _peerId;
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
                Exception lastException;
                lock (this)
                {
                    lastException = _lastSendException.Value ?? _lastReceiveException.Value;
                }
                if (lastException == null)
                    lastException = _communicationStrategy.LastException;
                return lastException;
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
                lock (this)
                {
                    _lastSendException.Value = null;
                }

                int peerId = await GetPeerId();
                TimestampID timestamp = await _storageStrategy.LoadOutgoingTimestampAsync(peerId);
                MessagesToSend messagesToSend;
                while ((messagesToSend = await GetMessagesToSendAsync(timestamp)) != null)
                {
                    await _communicationStrategy.PostAsync(
                        messagesToSend.MessageBodies,
                        await _model.GetClientDatabaseGuidAsync(),
                        messagesToSend.UnpublishedMessages);
                    timestamp = messagesToSend.Timestamp;
                    await _storageStrategy.SaveOutgoingTimestampAsync(peerId, timestamp);
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
                int peerId = await GetPeerId();
                GetResultMemento result = await _model.GetMessageBodiesAsync(timestamp, peerId, unpublishedMessages);
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
                else if (_receiveState.Value == ReceiveState.Polling)
                {
                    return;
                }
            }

            Receive();
        }

        private async void Receive()
        {
            try
            {
                lock (this)
                {
                    _lastReceiveException.Value = null;
                }

                GetManyResultMemento messagesToReceive;
                int peerId = await GetPeerId();
                while ((messagesToReceive = await GetMessagesToReceiveAsync()) != null)
                {
                    lock (this)
                    {
                        if (_receiveState.Value == ReceiveState.Polling)
                            _receiveState.Value = ReceiveState.Receiving;
                    }
                    await _model.ReceiveMessage(messagesToReceive.MessageBody, peerId);
                    foreach (PivotMemento pivot in messagesToReceive.NewTimestamps)
                    {
                        await _storageStrategy.SaveIncomingTimestampAsync(peerId, pivot.PivotId, pivot.Timestamp);
                    }
                }
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }
        }

        private async Task<GetManyResultMemento> GetMessagesToReceiveAsync()
        {
            int peerId = await GetPeerId();
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
                await GetPivotsAsync(pivotTree, pivotIds, peerId);

                bool anyPivots = pivotIds.Any();
                if (anyPivots)
                {
                    // Get the incoming timestamps.
                    List<PivotMemento> pivots = new List<PivotMemento>();
                    foreach (FactID pivotId in pivotIds)
                    {
                        TimestampID timestamp = await _storageStrategy.LoadIncomingTimestampAsync(peerId, pivotId);
                        pivots.Add(new PivotMemento(pivotId, timestamp));
                    }

                    // Get the messages from the server.
                    Task<GetManyResultMemento> getManyTask = _communicationStrategy.GetManyAsync(
                        pivotTree,
                        pivots,
                        await _model.GetClientDatabaseGuidAsync());
                    GetManyResultMemento result = await ChangeStateAfterDelay(getManyTask);
                    if (result != null && result.MessageBody.Facts.Any())
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
            try
            {
                FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabaseId);
                int peerId = await GetPeerId();
                await _model.AddToFactTreeAsync(pivotTree, pivot.ID, peerId);
                await _communicationStrategy.NotifyAsync(
                    pivotTree,
                    pivot.ID,
                    await _model.GetClientDatabaseGuidAsync(),
                    text1,
                    text2);
            }
            catch (Exception x)
            {
                _lastSendException.Value = x;
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
                _lastSendException.Value = exception;
            }
        }

        private void OnReceiveError(Exception exception)
        {
            lock (this)
            {
                _receiveState.Value = ReceiveState.Idle;
                _lastReceiveException.Value = exception;
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
            try
            {
                if (_communicationStrategy.IsLongPolling)
                    await _communicationStrategy.InterruptAsync(
                        await _model.GetClientDatabaseGuidAsync());

                BeginReceiving();
            }
            catch (Exception x)
            {
                OnReceiveError(x);
            }
        }

        private async Task<GetManyResultMemento> ChangeStateAfterDelay(Task<GetManyResultMemento> getManyTask)
        {
            // If there is no need to switch to the polling state,
            // just execute the task.
            if (!_communicationStrategy.IsLongPolling ||
                _receiveState.Value != ReceiveState.Receiving)
                return await getManyTask;

            // After a couple seconds, switch to the polling state.
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task<GetManyResultMemento> switchToPollingTask = Task.Delay(2000)
                .ContinueWith<GetManyResultMemento>(t =>
                {
                    if ((!tokenSource.IsCancellationRequested))
                        SetReceiveStatePolling();
                    return null;
                });

            try
            {
                Task<GetManyResultMemento> task = await Task.WhenAny(
                    switchToPollingTask,
                    getManyTask);
                GetManyResultMemento result = await task;
                return result;
            }
            catch (AggregateException x)
            {
                throw x.InnerExceptions.First();
            }
        }

        private void SetReceiveStatePolling()
        {
            lock (this)
            {
                if (_receiveState.Value == ReceiveState.Receiving)
                    _receiveState.Value = ReceiveState.Polling;
            }
        }

        public async void MessageReceived(FactTreeMemento messageBody)
        {
            try
            {
                int peerId = await GetPeerId();
                await _model.ReceiveMessage(messageBody, peerId);
            }
            catch (Exception x)
            {
                OnReceiveError(x);
            }
        }
    }
}
