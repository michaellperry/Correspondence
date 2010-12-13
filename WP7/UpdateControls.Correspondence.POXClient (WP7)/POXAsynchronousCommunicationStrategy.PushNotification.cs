using System;
using System.Collections.Generic;
using Microsoft.Phone.Notification;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.POXClient.Contract;
using UpdateControls.Correspondence.Strategy;
using System.IO;

namespace UpdateControls.Correspondence.POXClient
{
    public partial class POXAsynchronousCommunicationStrategy
    {
        private string _channelName;
        private HttpNotificationChannel _httpChannel;
        private Dictionary<FactID, WindowsPhonePushSubscription> _subscriptionByFactId = new Dictionary<FactID, WindowsPhonePushSubscription>();
        private bool _receivedChannelUri = false;
        private bool _openPending = false;

        private object _monitor = new object();

        public IPushSubscription SubscribeForPush(FactTreeMemento pivotTree, FactID pivotId, Guid clientGuid)
        {
            lock (_monitor)
            {
                WindowsPhonePushSubscription subscription;
                if (!_subscriptionByFactId.TryGetValue(pivotId, out subscription))
                {
                    FactTree pivot = Translate.MementoToFactTree(pivotTree);
                    subscription = new WindowsPhonePushSubscription(_configuration, pivot, pivotId.key, clientGuid, _monitor);
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
                _httpChannel = HttpNotificationChannel.Find(_channelName);
                if (_httpChannel != null)
                {
                    _receivedChannelUri = true;
                    SubscribeToChannelEvents();
                }
                else
                {
                    //Create the channel
                    _httpChannel = new HttpNotificationChannel(_channelName, "UpdateControls.Correspondence");
                    SubscribeToChannelEvents();
                }
            }

            if (!_receivedChannelUri)
            {
                if (!_openPending)
                {
                    _openPending = true;
                    _httpChannel.Open();
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
                    FactTreeMemento factTreeMemento = ReadFactTreeFromStorage(factReader);
                    MessageReceived(factTreeMemento);
                }
            }
        }

        private static FactTreeMemento ReadFactTreeFromStorage(BinaryReader factReader)
        {
            FactTreeMemento factTreeMemento = new FactTreeMemento(0);
            int factCount = factReader.ReadInt32();
            for (int i = 0; i < factCount; i++)
            {
                factTreeMemento.Add(ReadFactFromStorage(factReader));
            }
            return factTreeMemento;
        }

        private static IdentifiedFactMemento ReadFactFromStorage(BinaryReader factReader)
        {
            long factId;
            string typeName;
            int version;
            short dataSize;
            byte[] data;
            short predecessorCount;

            factId = factReader.ReadInt64();
            typeName = factReader.ReadString();
            version = factReader.ReadInt32();
            dataSize = factReader.ReadInt16();
            data = dataSize > 0 ? factReader.ReadBytes(dataSize) : new byte[0];
            predecessorCount = factReader.ReadInt16();

            FactMemento factMemento = new FactMemento(new CorrespondenceFactType(typeName, version));
            factMemento.Data = data;
            for (short i = 0; i < predecessorCount; i++)
            {
                string declaringTypeName;
                int declaringTypeVersion;
                string roleName;
                bool isPivot;
                long predecessorFactId;

                declaringTypeName = factReader.ReadString();
                declaringTypeVersion = factReader.ReadInt32();
                roleName = factReader.ReadString();
                isPivot = factReader.ReadBoolean();
                predecessorFactId = factReader.ReadInt64();

                factMemento.AddPredecessor(
                    new RoleMemento(
                        new CorrespondenceFactType(declaringTypeName, declaringTypeVersion),
                        roleName,
                        null,
                        isPivot),
                    new FactID() { key = predecessorFactId }
                );
            }

            return new IdentifiedFactMemento(new FactID { key = factId }, factMemento);
        }
    }
}