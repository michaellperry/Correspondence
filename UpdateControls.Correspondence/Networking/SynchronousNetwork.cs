using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Networking
{
	internal class SynchronousNetwork
	{
		private const long ClientDatabaseId = 0;

		private ISubscriptionProvider _subscriptionProvider;
		private Model _model;
		private IStorageStrategy _storageStrategy;

		private List<ICommunicationStrategy> _communicationStrategies = new List<ICommunicationStrategy>();

		public SynchronousNetwork(ISubscriptionProvider subscriptionProvider, Model model, IStorageStrategy storageStrategy)
		{
			_subscriptionProvider = subscriptionProvider;
			_model = model;
			_storageStrategy = storageStrategy;
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
				IEnumerable<FactTreeMemento> messageBodies = _model.GetMessageBodies(ref timestamp);
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
						FactTreeMemento messageBody = communicationStrategy.Get(pivotTree, pivotId, timestamp);
						if (messageBody.Facts.Any())
						{
							timestamp = _model.ReceiveMessage(messageBody);
							_storageStrategy.SaveIncomingTimestamp(protocolName, peerName, pivotId, timestamp);
							any = true;
						}
					}
				}
			}

			return any;
		}
	}
}
