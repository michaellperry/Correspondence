using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Correspondence.Strategy;
using Correspondence.WorkQueues;

namespace Correspondence.Networking
{
    class Network : ISubscriptionProvider
    {
        private Model _model;
        private IStorageStrategy _storageStrategy;
        private List<Subscription> _subscriptions = new List<Subscription>();

		private SynchronousNetwork _synchronousNetwork;
		private AsynchronousNetwork _asynchronousNetwork;

        public Network(Model model, IStorageStrategy storageStrategy, IWorkQueue workQueue, IWorkQueue asyncNetworkWorkQueue)
        {
            _model = model;
            _storageStrategy = storageStrategy;

            _synchronousNetwork = new SynchronousNetwork(this, _model, _storageStrategy, workQueue);
            _asynchronousNetwork = new AsynchronousNetwork(this, _model, _storageStrategy, asyncNetworkWorkQueue);
        }

        public void Subscribe(Subscription subscription)
        {
            _subscriptions.Add(subscription);
        }

		public IEnumerable<Subscription> Subscriptions
		{
			get { return _subscriptions; }
		}

        public void AddCommunicationStrategy(ICommunicationStrategy communicationStrategy)
        {
			_synchronousNetwork.AddCommunicationStrategy(communicationStrategy);
		}

        public void AddAsynchronousCommunicationStrategy(IAsynchronousCommunicationStrategy asynchronousCommunicationStrategy)
        {
			_asynchronousNetwork.AddAsynchronousCommunicationStrategy(asynchronousCommunicationStrategy);
		}

		public void BeginReceiving()
		{
			_asynchronousNetwork.BeginReceiving();
		}

        public void BeginSending()
        {
            _asynchronousNetwork.BeginSending();
        }

        public async Task<bool> SynchronizeAsync()
        {
			return await _synchronousNetwork.SynchronizeAsync();
		}

        public bool Synchronizing
        {
            get { return _asynchronousNetwork.Synchronizing || _synchronousNetwork.Synchronizing; }
        }

        public Exception LastException
        {
            get { return _asynchronousNetwork.LastException; }
        }

        public void Notify(CorrespondenceFact pivot, string text1, string text2)
        {
            _asynchronousNetwork.Notify(pivot, text1, text2);
        }
	}
}
