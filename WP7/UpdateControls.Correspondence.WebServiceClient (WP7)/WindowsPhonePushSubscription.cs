using UpdateControls.Correspondence.Strategy;
using System;

namespace UpdateControls.Correspondence.WebServiceClient
{
    public class WindowsPhonePushSubscription : IPushSubscription
    {
        private FactTree _pivot;
        private long _pivotId;
        private string _subscribedTo = null;
        private bool _callPending = false;

        private object _monitor;

        public WindowsPhonePushSubscription(FactTree pivot, long pivotId, object monitor)
        {
            _pivot = pivot;
            _pivotId = pivotId;
            _monitor = monitor;
        }

        public bool ShouldBeSubscribed { get; set; }

        public void UpdateSubscription(string deviceUri)
        {
            lock (_monitor)
            {
                if (!_callPending)
                {
                    if ((_subscribedTo != null && !ShouldBeSubscribed) || (_subscribedTo != deviceUri))
                    {
                        IWindowsPhonePushService windowsPhonePushService = new WindowsPhonePushServiceClient();
                        _callPending = true;
                        windowsPhonePushService.BeginUnsubscribe(
                            _pivot,
                            _pivotId,
                            _subscribedTo,
                            delegate(IAsyncResult a)
                            {
                                try
                                {
                                    windowsPhonePushService.EndUnsubscribe(a);
                                    UnsubscribeSuccess();
                                }
                                catch (Exception ex)
                                {
                                    UnsubscribeError();
                                    HandleException(ex);
                                }
                            },
                            null);
                    }

                    if (_subscribedTo == null && ShouldBeSubscribed)
                    {
                        IWindowsPhonePushService windowsPhonePushService = new WindowsPhonePushServiceClient();
                        _callPending = true;
                        windowsPhonePushService.BeginSubscribe(
                            _pivot,
                            _pivotId,
                            deviceUri,
                            delegate(IAsyncResult a)
                            {
                                try
                                {
                                    windowsPhonePushService.EndSubscribe(a);
                                    SubscribeSuccess(deviceUri);
                                }
                                catch (Exception ex)
                                {
                                    SubscribeError();
                                    HandleException(ex);
                                }
                            },
                            null);
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

        private void SubscribeError()
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
            }
        }

        private void UnsubscribeError()
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

        private void HandleException(Exception ex)
        {
            // TODO: Notify the application of an exception.
        }
    }
}
