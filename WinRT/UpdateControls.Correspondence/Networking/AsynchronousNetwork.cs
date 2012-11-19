using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Strategy;
using Windows.UI.Core;

namespace UpdateControls.Correspondence.Networking
{
	class AsynchronousNetwork
	{
		private ISubscriptionProvider _subscriptionProvider;
		private Model _model;
		private IStorageStrategy _storageStrategy;

        private List<AsynchronousServerProxy> _serverProxies = new List<AsynchronousServerProxy>();

		private Dependent _depPushSubscriptions;

        private CoreDispatcher _dispatcher;

        public AsynchronousNetwork(ISubscriptionProvider subscriptionProvider, Model model, IStorageStrategy storageStrategy)
        {
            _subscriptionProvider = subscriptionProvider;
            _model = model;
            _storageStrategy = storageStrategy;

            _depPushSubscriptions = new Dependent(UpdatePushSubscriptions);
            _depPushSubscriptions.Invalidated += TriggerSubscriptionUpdate;

            CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
            if (coreWindow != null)
                _dispatcher = coreWindow.Dispatcher;
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

        private async void RunOnUIThread(Action action)
        {
            if (_dispatcher != null)
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    delegate { action(); });
            else
                action();
        }
    }
}
