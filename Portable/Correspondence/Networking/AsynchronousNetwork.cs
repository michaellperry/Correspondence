using System;
using System.Collections.Generic;
using System.Linq;
using Assisticant.Collections;
using Correspondence.Strategy;
using Assisticant.Fields;
using Correspondence.WorkQueues;
using Assisticant;

namespace Correspondence.Networking
{
	class AsynchronousNetwork
	{
        private readonly ISubscriptionProvider _subscriptionProvider;
		private readonly Model _model;
		private readonly IStorageStrategy _storageStrategy;
        private readonly IWorkQueue _outgoingWorkQueue;
        private readonly IWorkQueue _incomingWorkQueue;

        private ObservableList<AsynchronousServerProxy> _serverProxies =
            new ObservableList<AsynchronousServerProxy>();
        private Observable<Exception> _lastException =
            new Observable<Exception>();

		private Computed _depPushSubscriptions;
  
        public AsynchronousNetwork(
            ISubscriptionProvider subscriptionProvider, 
            Model model, 
            IStorageStrategy storageStrategy,
            IWorkQueue outgoingWorkQueue,
            IWorkQueue incomingWorkQueue)
        {
            _subscriptionProvider = subscriptionProvider;
            _model = model;
            _storageStrategy = storageStrategy;
            _outgoingWorkQueue = outgoingWorkQueue;
            _incomingWorkQueue = incomingWorkQueue;

            _depPushSubscriptions = new Computed(UpdatePushSubscriptions);
            _depPushSubscriptions.Invalidated += delegate
            {
                UpdateScheduler.ScheduleUpdate(() => UpdateNow());
            };
        }

        public void AddAsynchronousCommunicationStrategy(IAsynchronousCommunicationStrategy asynchronousCommunicationStrategy)
        {
            AsynchronousServerProxy proxy = new AsynchronousServerProxy(
                _subscriptionProvider,
                _model,
                _storageStrategy,
                asynchronousCommunicationStrategy,
                _outgoingWorkQueue,
                _incomingWorkQueue);
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

        private void UpdateNow()
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
