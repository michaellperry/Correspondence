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

        private AwaitableCriticalSection _lock = new AwaitableCriticalSection();

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

        private async void Send()
        {
            using (await _lock.EnterAsync())
            {
                TimestampID timestamp = await _storageStrategy.LoadOutgoingTimestampAsync(_peerId);
                List<UnpublishMemento> unpublishedMessages = new List<UnpublishMemento>();
                var result = await _model.GetMessageBodiesAsync(timestamp, _peerId, unpublishedMessages);
                timestamp = result.Timestamp;
                FactTreeMemento messageBodies = result.FactTree;
                bool anyMessages = messageBodies != null && messageBodies.Facts.Any();
                if (anyMessages)
                {
                    try
                    {
                        await _communicationStrategy.PostAsync(
                            messageBodies,
                            await _model.GetClientDatabaseGuidAsync(),
                            unpublishedMessages);
                        await _storageStrategy.SaveOutgoingTimestampAsync(_peerId, timestamp);
                        Send();
                    }
                    catch (Exception e)
                    {
                        OnSendError(e);
                    }
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

        private async void Receive()
        {
            using (await _lock.EnterAsync())
            {
                FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabaseId);
                List<FactID> pivotIds = new List<FactID>();
                _depPushSubscriptions.OnGet();
                await GetPivotsAsync(pivotTree, pivotIds, _peerId);

                bool anyPivots = pivotIds.Any();
                if (anyPivots)
                {
                    List<PivotMemento> pivots = new List<PivotMemento>();
                    foreach (FactID pivotId in pivotIds)
                    {
                        TimestampID timestamp = await _storageStrategy.LoadIncomingTimestampAsync(_peerId, pivotId);
                        pivots.Add(new PivotMemento(pivotId, timestamp));
                    }
                    try
                    {
                        GetManyResultMemento result = await _communicationStrategy.GetManyAsync(
                            pivotTree,
                            pivots,
                            await _model.GetClientDatabaseGuidAsync());

                        bool receivedFacts = result.MessageBody.Facts.Any();
                        if (receivedFacts)
                        {
                            _model.ReceiveMessage(result.MessageBody, _peerId);
                            foreach (PivotMemento pivot in result.NewTimestamps)
                            {
                                await _storageStrategy.SaveIncomingTimestampAsync(_peerId, pivot.PivotId, pivot.Timestamp);
                            }
                        }
                        if (receivedFacts || _communicationStrategy.IsLongPolling)
                            Receive();
                        else
                        {
                            if (EndReceiving())
                                Receive();
                        }
                    }
                    catch (Exception e)
                    {
                        OnReceiveError(e);
                    }
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

        public async void AfterTriggerSubscriptionUpdate()
        {
            if (_communicationStrategy.IsLongPolling)
                await _communicationStrategy.InterruptAsync(
                    await _model.GetClientDatabaseGuidAsync());

            BeginReceiving();
        }
	}
}
