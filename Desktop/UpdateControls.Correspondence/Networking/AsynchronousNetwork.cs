using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Networking
{
	partial class AsynchronousNetwork
	{
		private const long ClientDatabaseId = 0;

		private ISubscriptionProvider _subscriptionProvider;
		private Model _model;
		private IStorageStrategy _storageStrategy;

        private List<AsynchronousServerProxy> _serverProxies = new List<AsynchronousServerProxy>();
		private Queue<SynchronizeResult> _synchronizeQueue = new Queue<SynchronizeResult>();

		private List<PushSubscriptionProxy> _pushSubscriptions = new List<PushSubscriptionProxy>();
		private Dependent _depPushSubscriptions;

		public AsynchronousNetwork(ISubscriptionProvider subscriptionProvider, Model model, IStorageStrategy storageStrategy)
		{
			_subscriptionProvider = subscriptionProvider;
			_model = model;
			_storageStrategy = storageStrategy;

			_depPushSubscriptions = new Dependent(UpdatePushSubscriptions);
			_depPushSubscriptions.Invalidated += TriggerAsync;
		}

		public void AddAsynchronousCommunicationStrategy(IAsynchronousCommunicationStrategy asynchronousCommunicationStrategy)
		{
            int peerId = _storageStrategy.SavePeer(
                asynchronousCommunicationStrategy.ProtocolName,
                asynchronousCommunicationStrategy.PeerName);
            asynchronousCommunicationStrategy.MessageReceived += messageBody =>
                _model.ReceiveMessage(messageBody, peerId);
            _serverProxies.Add(new AsynchronousServerProxy
            {
                CommunicationStrategy = asynchronousCommunicationStrategy,
                PeerId = peerId
            });
		}

		public void BeginSynchronize(AsyncCallback callback, object state)
		{
			if (!_serverProxies.Any())
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
				BeginSynchronize(result);
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
                    BeginSynchronize(result);
                }
			}
		}

        private void BeginSynchronize(SynchronizeResult result)
        {
            _depPushSubscriptions.OnGet();
            BeginSynchronizeOutgoing(result);
            BeginSynchronizeIncoming(result);
        }

        private void BeginSynchronizeOutgoing(SynchronizeResult result)
		{
			bool any = false;
			ResultAggregate communicationStrategyAggregate = new ResultAggregate(delegate()
			{
				result.OutgoingFinished(any);
			});
			foreach (AsynchronousServerProxy serverProxy in _serverProxies)
			{
				TimestampID timestamp = _storageStrategy.LoadOutgoingTimestamp(serverProxy.PeerId);
				FactTreeMemento messageBodies = _model.GetMessageBodies(ref timestamp, serverProxy.PeerId);
				if (messageBodies != null && messageBodies.Facts.Any())
				{
					any = true;
					communicationStrategyAggregate.Begin();
                    serverProxy.CommunicationStrategy.BeginPost(messageBodies, _model.ClientDatabaseGuid, delegate(bool succeeded)
                    {
						if (succeeded)
	                        _storageStrategy.SaveOutgoingTimestamp(serverProxy.PeerId, timestamp);
                        communicationStrategyAggregate.End();
                    });
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
			foreach (AsynchronousServerProxy serverProxy in _serverProxies)
			{
				foreach (Subscription subscription in _subscriptionProvider.Subscriptions)
				{
					foreach (CorrespondenceFact pivot in subscription.Pivots)
					{
						if (pivot == null)
							continue;

						FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabaseId);
						FactID pivotId = pivot.ID;
						_model.AddToFactTree(pivotTree, pivotId);
						TimestampID timestamp = _storageStrategy.LoadIncomingTimestamp(serverProxy.PeerId, pivotId);
						communicationStragetyAggregate.Begin();
						serverProxy.CommunicationStrategy.BeginGet(pivotTree, pivotId, timestamp, _model.ClientDatabaseGuid, delegate(FactTreeMemento messageBody, TimestampID newTimestamp)
						{
							if (messageBody.Facts.Any())
							{
								any = true;
                                _model.ReceiveMessage(messageBody, serverProxy.PeerId);
								lock (this)
								{
								    _storageStrategy.SaveIncomingTimestamp(serverProxy.PeerId, pivotId, newTimestamp);
								}
							}
							communicationStragetyAggregate.End();
						});
					}
				}
			}

			communicationStragetyAggregate.Close();
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

        private void TriggerAsync()
        {
            GetDispatcher().BeginInvoke(new Action(delegate
            {
                BeginSynchronize(a =>
                {
                    if (EndSynchronize(a))
                        TriggerAsync();
                }, null);
            }));
        }
    }
}
