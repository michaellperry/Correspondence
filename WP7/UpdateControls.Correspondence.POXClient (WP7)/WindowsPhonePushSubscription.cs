using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.POXClient.Contract;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.POXClient
{
    public class WindowsPhonePushSubscription : IPushSubscription
    {
        private static XmlSerializerNamespaces DefaultNamespaces = DefineNamespaces();
        private static XmlSerializer SubscribeRequestSerializer = new XmlSerializer(typeof(SubscribeRequest));
        private static XmlSerializer SubscribeResponseSerializer = new XmlSerializer(typeof(SubscribeResponse));
        private static XmlSerializer UnsubscribeRequestSerializer = new XmlSerializer(typeof(UnsubscribeRequest));
        private static XmlSerializer UnsubscribeResponseSerializer = new XmlSerializer(typeof(UnsubscribeResponse));

        private POXConfiguration _configuration;

        private FactTree _pivot;
        private long _pivotId;
        private Guid _clientGuid;
        private string _subscribedTo = null;
        private bool _callPending = false;

        private object _monitor;

        public WindowsPhonePushSubscription(POXConfiguration configuration, FactTree pivot, long pivotId, Guid clientGuid, object monitor)
        {
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
                    if ((_subscribedTo != null && !ShouldBeSubscribed) || (_subscribedTo != deviceUri))
                    {
                        _callPending = true;
                        UnsubscribeRequest request = new UnsubscribeRequest
                        {
                            PivotTree = _pivot,
                            PivotId = _pivotId,
                            DeviceUri = deviceUri
                        };
                        WebRequest webRequest = WebRequest.Create(new Uri(_configuration.Endpoint));
                        webRequest.Method = "POST";
                        webRequest.BeginGetRequestStream(a1 =>
                        {
                            try
                            {
                                Stream requestStream = webRequest.EndGetRequestStream(a1);
                                UnsubscribeRequestSerializer.Serialize(requestStream, request, DefaultNamespaces);
                                webRequest.BeginGetResponse(a2 =>
                                {
                                    try
                                    {
                                        WebResponse webResponse = webRequest.EndGetResponse(a2);
                                        Stream responseStream = webResponse.GetResponseStream();
                                        UnsubscribeResponse response = (UnsubscribeResponse)UnsubscribeResponseSerializer.Deserialize(responseStream);
                                        UnsubscribeSuccess();
                                    }
                                    catch (Exception ex)
                                    {
                                        UnsubscribeError();
                                        HandleException(ex);
                                    }
                                }, null);
                            }
                            catch (Exception ex)
                            {
                                UnsubscribeError();
                                HandleException(ex);
                            }
                        }, null);
                    }

                    if (_subscribedTo == null && ShouldBeSubscribed)
                    {
                        _callPending = true;
                        SubscribeRequest request = new SubscribeRequest
                        {
                            PivotTree = _pivot,
                            PivotId = _pivotId,
                            DeviceUri = deviceUri,
                            ClientGuid = _clientGuid.ToString()
                        };
                        WebRequest webRequest = WebRequest.Create(new Uri(_configuration.Endpoint));
                        webRequest.Method = "POST";
                        webRequest.BeginGetRequestStream(a1 =>
                        {
                            try
                            {
                                Stream requestStream = webRequest.EndGetRequestStream(a1);
                                SubscribeRequestSerializer.Serialize(requestStream, request, DefaultNamespaces);
                                webRequest.BeginGetResponse(a2 =>
                                {
                                    try
                                    {
                                        WebResponse webResponse = webRequest.EndGetResponse(a2);
                                        Stream responseStream = webResponse.GetResponseStream();
                                        SubscribeResponse response = (SubscribeResponse)SubscribeResponseSerializer.Deserialize(responseStream);
                                        SubscribeSuccess(deviceUri);
                                    }
                                    catch (Exception ex)
                                    {
                                        SubscribeError();
                                        HandleException(ex);
                                    }
                                }, null);
                            }
                            catch (Exception ex)
                            {
                                SubscribeError();
                                HandleException(ex);
                            }
                        }, null);
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

        private static XmlSerializerNamespaces DefineNamespaces()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("c", "http://correspondence.updatecontrols.com/pox/1.0");
            return namespaces;
        }
    }
}
