using System.Collections.Generic;
using System.Linq;
using Correspondence.Mementos;
using Correspondence.Strategy;
using System;
using Assisticant.Fields;

namespace Correspondence.Networking
{
    internal class ServerProxy
    {
        public ICommunicationStrategy CommunicationStrategy;
        public int PeerId;
    }
	internal class SynchronousNetwork
	{
		private const long ClientDatabaseId = 0;

		private ISubscriptionProvider _subscriptionProvider;
		private Model _model;
		private IStorageStrategy _storageStrategy;

		private List<ServerProxy> _serverProxies = new List<ServerProxy>();

        private Observable<bool> _synchronizing = new Observable<bool>();

		public SynchronousNetwork(ISubscriptionProvider subscriptionProvider, Model model, IStorageStrategy storageStrategy)
		{
			_subscriptionProvider = subscriptionProvider;
			_model = model;
			_storageStrategy = storageStrategy;
		}

		public void AddCommunicationStrategy(ICommunicationStrategy communicationStrategy)
		{
            _serverProxies.Add(new ServerProxy
            {
                CommunicationStrategy = communicationStrategy,
                PeerId = _storageStrategy.SavePeer(communicationStrategy.ProtocolName, communicationStrategy.PeerName)
            });
		}

		public bool Synchronize()
		{
            Synchronizing = true;
            try
            {
                bool any = false;
                if (SynchronizeOutgoing())
                    any = true;
                if (SynchronizeIncoming())
                    any = true;
                return any;
            }
            finally
            {
                Synchronizing = false;
            }
		}

        public bool Synchronizing
        {
            get
            {
                lock (this)
                {
                    return _synchronizing;
                }
            }
            private set
            {
                lock (this)
                {
                    _synchronizing.Value = value;
                }
            }
        }

        private bool SynchronizeOutgoing()
        {
            bool any = false;
            foreach (ServerProxy serverProxy in _serverProxies)
            {
                List<UnpublishMemento> unpublishedMessages = new List<UnpublishMemento>();
                TimestampID timestamp = _storageStrategy.LoadOutgoingTimestamp(serverProxy.PeerId);
                FactTreeMemento messageBodies = _model.GetMessageBodies(ref timestamp, serverProxy.PeerId, unpublishedMessages);
                if (messageBodies != null && messageBodies.Facts.Any())
                {
                    serverProxy.CommunicationStrategy.Post(messageBodies, unpublishedMessages);
                    _storageStrategy.SaveOutgoingTimestamp(serverProxy.PeerId, timestamp);
                    any = true;
                }
            }

            return any;
        }

        private bool SynchronizeIncoming()
        {
            bool any = false;
            foreach (ServerProxy serverProxy in _serverProxies)
            {
                foreach (Subscription subscription in _subscriptionProvider.Subscriptions)
                {
                    foreach (CorrespondenceFact pivot in subscription.Pivots)
                    {
                        if (pivot == null)
                            continue;

                        FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabaseId);
                        FactID pivotId = pivot.ID;
                        _model.AddToFactTree(pivotTree, pivotId, serverProxy.PeerId);
                        TimestampID timestamp = _storageStrategy.LoadIncomingTimestamp(serverProxy.PeerId, pivotId);
                        GetResultMemento result = serverProxy.CommunicationStrategy.Get(pivotTree, pivotId, timestamp);
                        if (result.FactTree.Facts.Any())
                        {
                            _model.ReceiveMessage(result.FactTree, serverProxy.PeerId);
                            _storageStrategy.SaveIncomingTimestamp(serverProxy.PeerId, pivotId, result.Timestamp);
                            any = true;
                        }
                    }
                }
            }

            return any;
        }
	}
}
