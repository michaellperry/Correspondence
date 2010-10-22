using UpdateControls.Correspondence.Strategy;
using System;

namespace UpdateControls.Correspondence.WebServiceClient
{
    public class WindowsPhonePushSubscription : IPushSubscription
    {
        private FactTree _pivot;
        private long _pivotId;
        private string _deviceUri;
        private bool _doNotSubscribe = false;

        public WindowsPhonePushSubscription(FactTree pivot, long pivotId)
        {
            _pivot = pivot;
            _pivotId = pivotId;
        }

        public void Subscribe(string deviceUri, Action subscriptionSuccess)
        {
            lock (this)
            {
                if (!_doNotSubscribe)
                {
                    _deviceUri = deviceUri;
                    IWindowsPhonePushService windowsPhonePushService = new WindowsPhonePushServiceClient();
                    windowsPhonePushService.BeginSubscribe(
                        _pivot,
                        _pivotId,
                        _deviceUri,
                        delegate(IAsyncResult a)
                        {
                            try
                            {
                                windowsPhonePushService.EndSubscribe(a);
                                subscriptionSuccess();
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                        },
                        null);
                }
            }
        }

        public void Unsubscribe()
        {
            lock (this)
            {
                if (_deviceUri != null)
                {
                    IWindowsPhonePushService windowsPhonePushService = new WindowsPhonePushServiceClient();
                    windowsPhonePushService.BeginUnsubscribe(
                        _pivot, 
                        _pivotId, 
                        _deviceUri,
                        delegate(IAsyncResult a)
                        {
                            try
                            {
                                windowsPhonePushService.EndUnsubscribe(a);
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                        }, 
                        null);
                }
                _doNotSubscribe = true;
            }
        }

        private void HandleException(Exception ex)
        {
            // TODO: Notify the application of an exception.
        }
    }
}
