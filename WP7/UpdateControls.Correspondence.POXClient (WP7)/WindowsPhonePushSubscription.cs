using System;
using UpdateControls.Correspondence.POXClient.Contract;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.POXClient
{
    public class WindowsPhonePushSubscription : IPushSubscription
    {
        private POXConfiguration _configuration;

        private FactTree _pivot;
        private long _pivotId;
        private Guid _clientGuid;
        private string _subscribedTo = null;
        private bool _callPending = false;

        private object _monitor;
        private Action _updateSubscriptions;

        public WindowsPhonePushSubscription(POXConfiguration configuration, FactTree pivot, long pivotId, Guid clientGuid, object monitor, Action updateSubscriptions)
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
                        UnsubscribeRequest request = new UnsubscribeRequest
                        {
                            Domain = _configuration.APIKey,
                            PivotTree = _pivot,
                            PivotId = _pivotId,
                            DeviceUri = deviceUri
                        };
                        POXHttpRequest.Begin<UnsubscribeRequest, UnsubscribeResponse>(
                            _configuration,
                            request,
                            response => UnsubscribeSuccess(),
                            ex => UnsubscribeError(ex));
                    }

                    else if (_subscribedTo == null && ShouldBeSubscribed)
                    {
                        _callPending = true;
                        SubscribeRequest request = new SubscribeRequest
                        {
                            Domain = _configuration.APIKey,
                            PivotTree = _pivot,
                            PivotId = _pivotId,
                            DeviceUri = deviceUri,
                            ClientGuid = _clientGuid.ToString()
                        };
                        POXHttpRequest.Begin<SubscribeRequest, SubscribeResponse>(
                            _configuration,
                            request,
                            response => SubscribeSuccess(deviceUri),
                            ex => SubscribeError(ex));
                    }
                }
            }
        }

        private void SubscribeSuccess(string deviceUri)
        {
            lock (_monitor)
            {
                _callPending = false;
                _subscribedTo = deviceUri;
            }
        }

        private void SubscribeError(Exception ex)
        {
            lock (_monitor)
            {
                _callPending = false;
            }
        }

        private void UnsubscribeSuccess()
        {
            lock (_monitor)
            {
                _callPending = false;
                _subscribedTo = null;
                _updateSubscriptions();
            }
        }

        private void UnsubscribeError(Exception ex)
        {
            lock (_monitor)
            {
                _callPending = false;
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
