using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Networking
{
	partial class AsynchronousNetwork
	{
		private ISubscriptionProvider _subscriptionProvider;
		private Model _model;
		private IStorageStrategy _storageStrategy;

        private List<AsynchronousServerProxy> _serverProxies = new List<AsynchronousServerProxy>();

		private Dependent _depPushSubscriptions;

		public AsynchronousNetwork(ISubscriptionProvider subscriptionProvider, Model model, IStorageStrategy storageStrategy)
		{
			_subscriptionProvider = subscriptionProvider;
			_model = model;
			_storageStrategy = storageStrategy;

			_depPushSubscriptions = new Dependent(UpdatePushSubscriptions);
			_depPushSubscriptions.Invalidated += TriggerSubscriptionUpdate;
		}

		public void AddAsynchronousCommunicationStrategy(IAsynchronousCommunicationStrategy asynchronousCommunicationStrategy)
		{
            int peerId = _storageStrategy.SavePeer(
                asynchronousCommunicationStrategy.ProtocolName,
                asynchronousCommunicationStrategy.PeerName);
            asynchronousCommunicationStrategy.MessageReceived += messageBody =>
            {
                _model.ReceiveMessage(messageBody, peerId);
                // Trigger a receive on normal channels. This updates the
                // timestamp and pulls down any messages that were too long
                // for the push buffer.
                BeginReceiving();
            };
            _serverProxies.Add(new AsynchronousServerProxy(
                _subscriptionProvider, 
                _model, 
                _storageStrategy, 
                asynchronousCommunicationStrategy, 
                peerId));
		}

        public bool Synchronizing
        {
            get { return _serverProxies.Any(serverProxy => serverProxy.Synchronizing); }
        }

        public Exception LastException
        {
            get
            {
                return _serverProxies
                    .Select(serverProxy => serverProxy.LastException)
                    .FirstOrDefault(exception => exception != null);
            }
        }

        public void BeginSending()
        {
            _depPushSubscriptions.OnGet();
            foreach (var serverProxy in _serverProxies)
                serverProxy.BeginSending();
        }

        public void BeginReceiving()
        {
            _depPushSubscriptions.OnGet();
            foreach (var serverProxy in _serverProxies)
                serverProxy.BeginReceiving();
        }

        public void Notify(CorrespondenceFact pivot, string text1, string text2)
        {
            foreach (var serverProxy in _serverProxies)
                serverProxy.Notify(pivot, text1, text2);
        }

        private void UpdatePushSubscriptions()
		{
            foreach (var serverProxy in _serverProxies)
                serverProxy.TriggerSubscriptionUpdate();
		}

        private void TriggerSubscriptionUpdate()
        {
            RunOnUIThread(new Action(() =>
            {
                _depPushSubscriptions.OnGet();

                foreach (var serverProxy in _serverProxies)
                    serverProxy.AfterTriggerSubscriptionUpdate();
            }));
        }
    }
}
