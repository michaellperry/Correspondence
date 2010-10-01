using System;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using Microsoft.Phone.Notification;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.WebServiceClient
{
    public class WebServiceCommunicationStrategy : IAsynchronousCommunicationStrategy
    {
        private string _channelName;
        private HttpNotificationChannel _httpChannel;
        private List<WindowsPhonePushSubscription> _subscriptionQueue = new List<WindowsPhonePushSubscription>();

        public WebServiceCommunicationStrategy(string channelName)
        {
            _channelName = channelName;
        }

        public string ProtocolName
        {
            get { return "http://correspondence.updatecontrols.net"; }
        }

        public string PeerName
        {
            get { return new SynchronizationServiceClient().Endpoint.Address.ToString(); }
        }

        public void BeginGet(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp, Action<FactTreeMemento> callback)
        {
            FactTree pivot = Translate.MementoToFactTree(pivotTree);
            ISynchronizationService synchronizationService = new SynchronizationServiceClient();
            synchronizationService.BeginGet(pivot, pivotId.key, timestamp.Key, delegate(IAsyncResult result)
            {
                FactTree factTree = synchronizationService.EndGet(result);
                callback(Translate.FactTreeToMemento(factTree));
            }, null);
        }

        public void BeginPost(FactTreeMemento messageBody, Action callback)
        {
            ISynchronizationService synchronizationService = new SynchronizationServiceClient();
            synchronizationService.BeginPost(Translate.MementoToFactTree(messageBody), delegate(IAsyncResult result)
            {
                synchronizationService.EndPost(result);
                callback();
            }, null);
        }


        public IPushSubscription SubscribeForPush(FactTreeMemento pivotTree, FactID pivotId, Action<FactTreeMemento> callback)
        {
            lock (this)
            {
                FactTree pivot = Translate.MementoToFactTree(pivotTree);
                WindowsPhonePushSubscription subscription = new WindowsPhonePushSubscription(pivot, pivotId.key);
                _subscriptionQueue.Add(subscription);
                OpenChannel();
                return subscription;
            }
        }

        private void OpenChannel()
        {
            if (_httpChannel == null)
            {
                //First, try to pick up existing channel
                _httpChannel = HttpNotificationChannel.Find(_channelName);
                if (null != _httpChannel)
                {
                    SubscribeToChannelEvents();
                    SubscribeToService();
                }
                else
                {
                    //Create the channel
                    _httpChannel = new HttpNotificationChannel(_channelName, "HOLWeatherService");
                    SubscribeToChannelEvents();
                    _httpChannel.Open();
                }
            }
        }

        private void SubscribeToChannelEvents()
        {
            _httpChannel.ChannelUriUpdated += httpChannel_ChannelUriUpdated;
            _httpChannel.HttpNotificationReceived += httpChannel_HttpNotificationReceived;
            _httpChannel.ErrorOccurred += httpChannel_ExceptionOccurred;
        }

        private void SubscribeToService()
        {
            string deviceUri = _httpChannel.ChannelUri.ToString();
            foreach (WindowsPhonePushSubscription subscription in _subscriptionQueue)
            {
                subscription.Subscribe(deviceUri);
            }
        }

        void httpChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            lock (this)
            {
                SubscribeToService();
            }
        }

        void httpChannel_ExceptionOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
        }

        void httpChannel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
        }
    }
}
