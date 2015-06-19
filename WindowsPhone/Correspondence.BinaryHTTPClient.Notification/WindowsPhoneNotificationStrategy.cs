using Microsoft.Phone.Notification;
using System;
using System.Collections.Generic;
using System.IO;
using Correspondence.FieldSerializer;
using Correspondence.Mementos;
using Correspondence.Strategy;

namespace Correspondence.BinaryHTTPClient.Notification
{
    public class WindowsPhoneNotificationStrategy : INotificationStrategy
    {
        private ToastNotificationObserver _toastNotificationObserver;
        private HTTPConfiguration _configuration;

        private HttpNotificationChannel _httpChannel;
        private Dictionary<FactID, WindowsPhonePushSubscription> _subscriptionByFactId = new Dictionary<FactID, WindowsPhonePushSubscription>();
        private bool _receivedChannelUri = false;
        private bool _openPending = false;

        private object _monitor = new object();

        public WindowsPhoneNotificationStrategy(IHTTPConfigurationProvider configurationProvider)
        {
            _toastNotificationObserver = new ToastNotificationObserver(configurationProvider);
            _configuration = configurationProvider.Configuration;
        }

        public IPushSubscription SubscribeForPush(FactTreeMemento pivotTree, FactID pivotId, Guid clientGuid)
        {
            lock (_monitor)
            {
                WindowsPhonePushSubscription subscription;
                if (!_subscriptionByFactId.TryGetValue(pivotId, out subscription))
                {
                    subscription = new WindowsPhonePushSubscription(_configuration, pivotTree, pivotId.key, clientGuid, _monitor, UpdateSubscriptions);
                    _subscriptionByFactId.Add(pivotId, subscription);
                }
                subscription.ShouldBeSubscribed = true;
                UpdateSubscriptions();
                return subscription;
            }
        }

        private void UpdateSubscriptions()
        {
            if (_httpChannel == null)
            {
                //First, try to pick up existing channel
                _httpChannel = HttpNotificationChannel.Find(_configuration.ChannelName);
                if (_httpChannel != null)
                {
                    _receivedChannelUri = _httpChannel.ChannelUri != null;
                    _openPending = !_receivedChannelUri;
                    SubscribeToChannelEvents();
                    _toastNotificationObserver.HttpChannel = _httpChannel;
                }
                else
                {
                    //Create the channel
                    _httpChannel = new HttpNotificationChannel(_configuration.ChannelName, "Correspondence");
                    SubscribeToChannelEvents();
                }
            }

            if (!_receivedChannelUri)
            {
                if (!_openPending)
                {
                    _openPending = true;
                    _httpChannel.Open();
                    _toastNotificationObserver.HttpChannel = _httpChannel;
                }
            }
            else
            {
                string deviceUri = _httpChannel.ChannelUri.ToString();
                foreach (WindowsPhonePushSubscription subscription in _subscriptionByFactId.Values)
                {
                    subscription.UpdateSubscription(deviceUri);
                }
            }
        }

        private void SubscribeToChannelEvents()
        {
            _httpChannel.ChannelUriUpdated += httpChannel_ChannelUriUpdated;
            _httpChannel.HttpNotificationReceived += httpChannel_HttpNotificationReceived;
            _httpChannel.ErrorOccurred += httpChannel_ExceptionOccurred;
        }

        private void httpChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            lock (_monitor)
            {
                _receivedChannelUri = true;
                _openPending = false;
                UpdateSubscriptions();
            }
        }

        private void httpChannel_ExceptionOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            lock (_monitor)
            {
                _openPending = false;
            }
        }

        private void httpChannel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            if (MessageReceived != null)
            {
                using (BinaryReader factReader = new BinaryReader(e.Notification.Body))
                {
                    byte version = BinaryHelper.ReadByte(factReader);
                    if (version != 1)
                        throw new CorrespondenceException(String.Format("Unknown fact tree version {0}.", version));

                    FactTreeMemento factTreeMemento = new FactTreeSerlializer().DeserializeFactTree(factReader);
                    MessageReceived(factTreeMemento);
                }
            }
        }

        public event Action<FactTreeMemento> MessageReceived;

        public Exception LastException
        {
            get { return null; }
        }
    }
}
