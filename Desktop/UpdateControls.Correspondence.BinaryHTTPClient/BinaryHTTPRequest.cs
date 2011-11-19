using System;
using System.IO;
using System.Net;

namespace UpdateControls.Correspondence.BinaryHTTPClient
{
    public static class BinaryHTTPRequest
    {
        public class BinaryHTTPRequestHandler
        {
            private BinaryRequest _request;
            private WebRequest _webRequest;
            private Action<BinaryResponse> _success;
            private Action<Exception> _failure;

            public BinaryHTTPRequestHandler(
                HTTPConfiguration configuration,
                BinaryRequest request,
                Action<BinaryResponse> success,
                Action<Exception> failure)
            {
                _request = request;
                _success = success;
                _failure = failure;

                _webRequest = WebRequest.Create(new Uri(configuration.Endpoint, UriKind.Absolute));
                _webRequest.Method = "POST";
                _webRequest.ContentType = "application/octet-stream";
            }

            public void Begin()
            {
                _webRequest.BeginGetRequestStream(GetRequestStream, null);
            }

            private void GetRequestStream(IAsyncResult result)
            {
                try
                {
                    using (BinaryWriter requestWriter = new BinaryWriter(_webRequest.EndGetRequestStream(result)))
                    {
                        _request.Write(requestWriter);
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
                    BinaryResponse response;
                    using (BinaryReader responseReader = new BinaryReader(webResponse.GetResponseStream()))
                    {
                        response = BinaryResponse.Read(responseReader);
                    }
                    _success(response);
                }
                catch (Exception ex)
                {
                    _failure(ex);
                }
            }
        }

        public static void Begin(
            HTTPConfiguration configuration,
            BinaryRequest request,
            Action<BinaryResponse> success,
            Action<Exception> failure)
        {
            new BinaryHTTPRequestHandler(configuration, request, success, failure).Begin();
        }
    }
}
