﻿using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace UpdateControls.Correspondence.POXClient
{
    public static class POXHttpRequest
    {
        private static XmlSerializerNamespaces DefaultNamespaces = DefineNamespaces();

        public class POXHttpRequestHandler<TRequest, TResponse>
        {
            private static XmlSerializer GetRequestSerializer = new XmlSerializer(typeof(TRequest));
            private static XmlSerializer GetResponseSerializer = new XmlSerializer(typeof(TResponse));

            private TRequest _request;
            private WebRequest _webRequest;
            private Action<TResponse> _success;
            private Action<Exception> _failure;

            public POXHttpRequestHandler(
                string endpoint,
                TRequest request,
                Action<TResponse> success,
                Action<Exception> failure)
            {
                _request = request;
                _success = success;
                _failure = failure;

                _webRequest = WebRequest.Create(new Uri(endpoint, UriKind.Absolute));
                _webRequest.Method = "POST";
                _webRequest.ContentType = "text/xml";
            }

            private void Begin()
            {
                _webRequest.BeginGetRequestStream(GetRequestStream, null);
            }

            private void GetRequestStream(IAsyncResult result)
            {
                try
                {
                    using (Stream requestStream = _webRequest.EndGetRequestStream(result))
                    {
                        GetRequestSerializer.Serialize(requestStream, _request, DefaultNamespaces);
                    }
                    _webRequest.BeginGetResponse(GetResponse, null);
                }
                catch (Exception ex)
                {
                    _failure(ex);
                }
            }

            private void GetResponse(IAsyncResult result)
            {
                try
                {
                    WebResponse webResponse = _webRequest.EndGetResponse(result);
                    TResponse response;
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        response = (TResponse)GetResponseSerializer.Deserialize(responseStream);
                    }
                    _success(response);
                }
                catch (Exception ex)
                {
                    _failure(ex);
                }
            }
        }

        public static void Begin<TRequest, TResponse>(
            string endpoint,
            TRequest request,
            Action<TResponse> success,
            Action<Exception> failure)
        {
            var handler = new POXHttpRequestHandler<TRequest, TResponse>(endpoint, request, success, failure);
        }

        private static XmlSerializerNamespaces DefineNamespaces()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("c", "http://correspondence.updatecontrols.com/pox/1.0");
            return namespaces;
        }
    }
}
