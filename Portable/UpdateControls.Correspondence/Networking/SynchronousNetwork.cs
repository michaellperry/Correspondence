using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System;
using UpdateControls.Fields;
using System.Threading.Tasks;
using UpdateControls.Correspondence.WorkQueues;

namespace UpdateControls.Correspondence.Networking
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

        private Independent<bool> _synchronizing = new Independent<bool>();

        private AwaitableCriticalSection _lock = new AwaitableCriticalSection();
        private readonly IWorkQueue _workQueue;
  
		public SynchronousNetwork(
            ISubscriptionProvider subscriptionProvider,
            Model model,
            IStorageStrategy storageStrategy,
            IWorkQueue workQueue)
		{
            _subscriptionProvider = subscriptionProvider;
			_model = model;
			_storageStrategy = storageStrategy;
            _workQueue = workQueue;
        }

        public void AddCommunicationStrategy(ICommunicationStrategy communicationStrategy)
        {
            _workQueue.Perform(() => AddCommunicationStrategyAsync(communicationStrategy));
        }

		private async Task AddCommunicationStrategyAsync(ICommunicationStrategy communicationStrategy)
		{
            using (await _lock.EnterAsync())
            {
                _serverProxies.Add(new ServerProxy
                {
                    CommunicationStrategy = communicationStrategy,
                    PeerId = await _storageStrategy.SavePeerAsync(communicationStrategy.ProtocolName, communicationStrategy.PeerName)
                });
            }
		}

		public async Task<bool> SynchronizeAsync()
		{
            Synchronizing = true;
            try
            {
                bool any = false;
                if (await SynchronizeOutgoingAsync())
                    any = true;
                if (await SynchronizeIncomingAsync())
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

        private async Task<bool> SynchronizeOutgoingAsync()
        {
            bool any = false;
            foreach (ServerProxy serverProxy in _serverProxies)
            {
                List<UnpublishMemento> unpublishedMessages = new List<UnpublishMemento>();
                TimestampID timestamp = await _storageStrategy.LoadOutgoingTimestampAsync(serverProxy.PeerId);
                GetResultMemento result = await _model.GetMessageBodiesAsync(timestamp, serverProxy.PeerId, unpublishedMessages);
                timestamp = result.Timestamp;
                FactTreeMemento messageBodies = result.FactTree;
                if (messageBodies != null && messageBodies.Facts.Any())
                {
                    serverProxy.CommunicationStrategy.Post(messageBodies, unpublishedMessages);
                    await _storageStrategy.SaveOutgoingTimestampAsync(serverProxy.PeerId, timestamp);
                    any = true;
                }
            }

            return any;
        }

        private async Task<bool> SynchronizeIncomingAsync()
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
                        await _model.AddToFactTreeAsync(pivotTree, pivotId, serverProxy.PeerId);
                        TimestampID timestamp = await _storageStrategy.LoadIncomingTimestampAsync(serverProxy.PeerId, pivotId);
                        GetResultMemento result = await serverProxy.CommunicationStrategy.GetAsync(pivotTree, pivotId, timestamp);
                        if (result.FactTree.Facts.Any())
                        {
                            await _model.ReceiveMessage(result.FactTree, serverProxy.PeerId);
                            await _storageStrategy.SaveIncomingTimestampAsync(serverProxy.PeerId, pivotId, result.Timestamp);
                            any = true;
                        }
                    }
                }
            }

            return any;
        }
	}
}
