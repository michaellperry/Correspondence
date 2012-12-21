﻿using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System.IO;
using UpdateControls.Correspondence.FieldSerializer;
using Windows.Networking.PushNotifications;

namespace UpdateControls.Correspondence.BinaryHTTPClient.Notification
{
    public class WindowsNotificationStrategy : INotificationStrategy
    {
        private HTTPConfiguration _configuration;

        private Dictionary<FactID, WindowsPushSubscription> _subscriptionByFactId =
            new Dictionary<FactID, WindowsPushSubscription>();

        private object _monitor = new object();
        private bool _requestPending = false;

        public WindowsNotificationStrategy(IHTTPConfigurationProvider configurationProvider)
        {
            _configuration = configurationProvider.Configuration;
        }

        public IPushSubscription SubscribeForPush(FactTreeMemento pivotTree, FactID pivotId, Guid clientGuid)
        {
            lock (_monitor)
            {
                WindowsPushSubscription subscription;
                if (!_subscriptionByFactId.TryGetValue(pivotId, out subscription))
                {
                    subscription = new WindowsPushSubscription(_configuration, pivotTree, pivotId.key, clientGuid, _monitor, UpdateSubscriptions);
                    _subscriptionByFactId.Add(pivotId, subscription);
                }
                subscription.ShouldBeSubscribed = true;
                UpdateSubscriptions();
                return subscription;
            }
        }

        private async void UpdateSubscriptions()
        {
            lock (_monitor)
            {
                if (_requestPending)
                    return;

                _requestPending = true;
            }

            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            channel.PushNotificationReceived += PushNotificationReceived;
            var uri = channel.Uri;
            lock (_monitor)
            {
                _requestPending = false;
                foreach (var subscription in _subscriptionByFactId.Values)
                {
                    subscription.UpdateSubscription(uri);
                }
            }
        }

        private void PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            if (MessageReceived != null)
            {
                byte[] body = Convert.FromBase64String(args.RawNotification.Content);
                using (BinaryReader factReader = new BinaryReader(new MemoryStream(body)))
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
    }
}