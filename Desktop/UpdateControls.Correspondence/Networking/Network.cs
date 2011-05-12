using System;
using System.Collections.Generic;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Networking
{
    class Network : ISubscriptionProvider
    {
        private Model _model;
        private IStorageStrategy _storageStrategy;
        private List<Subscription> _subscriptions = new List<Subscription>();

		private SynchronousNetwork _synchronousNetwork;
		private AsynchronousNetwork _asynchronousNetwork;

		public Network(Model model, IStorageStrategy storageStrategy)
        {
            _model = model;
            _storageStrategy = storageStrategy;

			_synchronousNetwork = new SynchronousNetwork(this, _model, _storageStrategy);
			_asynchronousNetwork = new AsynchronousNetwork(this, _model, _storageStrategy);
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

		public void BeginSynchronize(AsyncCallback callback, object state)
		{
			_asynchronousNetwork.BeginSynchronize(callback, state);
		}

		public bool EndSynchronize(IAsyncResult asyncResult)
		{
			return _asynchronousNetwork.EndSynchronize(asyncResult);
		}

        public bool Synchronize()
        {
			return _synchronousNetwork.Synchronize();
		}

        public bool Synchronizing
        {
            get { return _asynchronousNetwork.Synchronizing || _synchronousNetwork.Synchronizing; }
        }

        public Exception LastException
        {
            get { return _asynchronousNetwork.LastException; }
        }
	}
}
