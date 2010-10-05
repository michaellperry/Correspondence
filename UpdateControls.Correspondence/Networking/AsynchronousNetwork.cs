using UpdateControls.Correspondence;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Strategy;
using System;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Networking
{
	class AsynchronousNetwork
	{
		private const long ClientDatabaseId = 0;

		private ISubscriptionProvider _subscriptionProvider;
		private Model _model;
		private IStorageStrategy _storageStrategy;

		private List<IAsynchronousCommunicationStrategy> _asynchronousCommunicationStrategies = new List<IAsynchronousCommunicationStrategy>();
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
			asynchronousCommunicationStrategy.MessageReceived += messageBody => _model.ReceiveMessage(messageBody);
			_asynchronousCommunicationStrategies.Add(asynchronousCommunicationStrategy);
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
				IEnumerable<FactTreeMemento> messageBodies = _model.GetMessageBodies(ref timestamp);
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

				foreach (Subscription subscription in _subscriptionProvider.Subscriptions)
				{
					foreach (CorrespondenceFact pivot in subscription.Pivots)
					{
						if (pivot == null)
							continue;

						FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabaseId, 0L);
						FactID pivotId = pivot.ID;
						_model.AddToFactTree(pivotTree, pivotId);
						TimestampID timestamp = _storageStrategy.LoadIncomingTimestamp(protocolName, peerName, pivotId);
						communicationStragetyAggregate.Begin();
						asynchronousCommunicationStrategy.BeginGet(pivotTree, pivotId, timestamp, delegate(FactTreeMemento messageBody)
						{
							if (messageBody.Facts.Any())
							{
								any = true;
								TimestampID newTimestamp = _model.ReceiveMessage(messageBody);
								_storageStrategy.SaveIncomingTimestamp(protocolName, peerName, pivotId, newTimestamp);
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
					from strategy in _asynchronousCommunicationStrategies
					from pivot in pivots
					select bin.Extract(new PushSubscriptionProxy(_model, strategy, pivot));

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
