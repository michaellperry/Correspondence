using System;
using System.Threading.Tasks;
using Correspondence.Mementos;
using Correspondence.Strategy;

namespace Correspondence.BinaryHTTPClient.Notification
{
    public class WindowsPushSubscription : IPushSubscription
    {
        private HTTPConfiguration _configuration;

        private FactTreeMemento _pivot;
        private long _pivotId;
        private Guid _clientGuid;
        private string _subscribedTo = null;
        private bool _callPending = false;

        private object _monitor;
        private Action _updateSubscriptions;

        public WindowsPushSubscription(HTTPConfiguration configuration, FactTreeMemento pivot, long pivotId, Guid clientGuid, object monitor, Action updateSubscriptions)
        {
            _updateSubscriptions = updateSubscriptions;
            _configuration = configuration;
            _pivot = pivot;
            _pivotId = pivotId;
            _clientGuid = clientGuid;
            _monitor = monitor;
        }

        public bool ShouldBeSubscribed { get; set; }

        public void UpdateSubscription(string deviceUri)
        {
            lock (_monitor)
            {
                if (!_callPending)
                {
                    if (_subscribedTo != null && (!ShouldBeSubscribed || _subscribedTo != deviceUri))
                    {
                        _callPending = true;
                        Task.Run(() => SendUnsubscribeAsync(deviceUri));
                    }

                    else if (_subscribedTo == null && ShouldBeSubscribed)
                    {
                        _callPending = true;
                        Task.Run(() => SendSubscribeAsync(deviceUri));
                    }
                }
            }
        }

        private async Task SendUnsubscribeAsync(string deviceUri)
        {
            try
            {
                WindowsUnsubscribeRequest request = new WindowsUnsubscribeRequest
                {
                    Domain = _configuration.APIKey,
                    PivotTree = _pivot,
                    PivotId = _pivotId,
                    DeviceUri = deviceUri
                };
                await BinaryHTTPRequest.SendAsync(_configuration, request);
                lock (_monitor)
                {
                    _callPending = false;
                    _subscribedTo = null;
                    _updateSubscriptions();
                }
            }
            catch (Exception x)
            {
                lock (_monitor)
                {
                    _callPending = false;
                }
            }
        }

        private async Task SendSubscribeAsync(string deviceUri)
        {
            try
            {
                WindowsSubscribeRequest request = new WindowsSubscribeRequest
                {
                    Domain = _configuration.APIKey,
                    PivotTree = _pivot,
                    PivotId = _pivotId,
                    DeviceUri = deviceUri,
                    ClientGuid = _clientGuid.ToString()
                };
                await BinaryHTTPRequest.SendAsync(_configuration, request);
                lock (_monitor)
                {
                    _callPending = false;
                    _subscribedTo = deviceUri;
                }
            }
            catch (Exception x)
            {
                lock (_monitor)
                {
                    _callPending = false;
                }
            }
        }

        public void Unsubscribe()
        {
            lock (_monitor)
            {
                ShouldBeSubscribed = false;
            }
        }
    }
}
