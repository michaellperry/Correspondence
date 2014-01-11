using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Collections;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Fields;

namespace UpdateControls.Correspondence.Networking
{
	class AsynchronousNetwork : IUpdatable
	{
        private ISubscriptionProvider _subscriptionProvider;
		private Model _model;
		private IStorageStrategy _storageStrategy;

        private IndependentList<AsynchronousServerProxy> _serverProxies =
            new IndependentList<AsynchronousServerProxy>();
        private Independent<Exception> _lastException =
            new Independent<Exception>();

		private Dependent _depPushSubscriptions;

        public AsynchronousNetwork(ISubscriptionProvider subscriptionProvider, Model model, IStorageStrategy storageStrategy)
        {
            _subscriptionProvider = subscriptionProvider;
            _model = model;
            _storageStrategy = storageStrategy;

            _depPushSubscriptions = new Dependent(UpdatePushSubscriptions);
            _depPushSubscriptions.Invalidated += delegate
            {
                UpdateScheduler.ScheduleUpdate(this);
            };
        }

        public void AddAsynchronousCommunicationStrategy(IAsynchronousCommunicationStrategy asynchronousCommunicationStrategy)
        {
            AsynchronousServerProxy proxy = new AsynchronousServerProxy(
                _subscriptionProvider,
                _model,
                _storageStrategy,
                asynchronousCommunicationStrategy);
            _serverProxies.Add(proxy);

            asynchronousCommunicationStrategy.MessageReceived += messageBody =>
            {
                try
                {
                    proxy.MessageReceived(messageBody);
                    // Trigger a receive on normal channels. This updates the
                    // timestamp and pulls down any messages that were too long
                    // for the push buffer.
                    BeginReceiving();
                }
                catch (Exception x)
                {
                    lock (this)
                    {
                        _lastException.Value = x;
                    }
                }
            };
        }

        public bool Synchronizing
        {
            get
            {
                lock (this)
                {
                    return _serverProxies.Any(serverProxy => serverProxy.Synchronizing);
                }
            }
        }

        public Exception LastException
        {
            get
            {
                lock (this)
                {
                    return _lastException.Value ?? _serverProxies
                        .Select(serverProxy => serverProxy.LastException)
                        .FirstOrDefault(exception => exception != null);
                }
            }
        }

        public void BeginSending()
        {
            lock (this)
            {
                _depPushSubscriptions.OnGet();
                foreach (var serverProxy in _serverProxies)
                    serverProxy.BeginSending();
            }
        }

        public void BeginReceiving()
        {
            lock (this)
            {
                _depPushSubscriptions.OnGet();
                foreach (var serverProxy in _serverProxies)
                    serverProxy.BeginReceiving();
            }
        }

        public void Notify(CorrespondenceFact pivot, string text1, string text2)
        {
            lock (this)
            {
                foreach (var serverProxy in _serverProxies)
                    serverProxy.Notify(pivot, text1, text2);
            }
        }

        private void UpdatePushSubscriptions()
		{
            foreach (var serverProxy in _serverProxies)
                serverProxy.TriggerSubscriptionUpdate();
		}

        public void UpdateNow()
        {
            lock (this)
            {
                _depPushSubscriptions.OnGet();

                foreach (var serverProxy in _serverProxies)
                    serverProxy.AfterTriggerSubscriptionUpdate();
            }
        }
    }
}
