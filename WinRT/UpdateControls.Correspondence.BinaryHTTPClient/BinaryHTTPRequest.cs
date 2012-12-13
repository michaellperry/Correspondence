using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.BinaryHTTPClient
{
    public static class BinaryHTTPRequest
    {
        public static async Task<BinaryResponse> SendAsync(
            HTTPConfiguration configuration,
            BinaryRequest request)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task<BinaryResponse> sendTask = InternalSendAsync(configuration, request, tokenSource);
            Task<BinaryResponse> timeoutTask = Task
                .Delay(configuration.TimeoutSeconds * 1500, tokenSource.Token)
                .ContinueWith<BinaryResponse>(delegate
                {
                    throw new TimeoutException();
                });
            var response = await Task.WhenAny<BinaryResponse>(sendTask, timeoutTask);
            tokenSource.Cancel();
            return await response;
        }

        private static async Task<BinaryResponse> InternalSendAsync(
            HTTPConfiguration configuration,
            BinaryRequest request,
            CancellationTokenSource tokenSource)
        {
            var _request = request;

            var _webRequest = WebRequest.Create(new Uri(configuration.Endpoint, UriKind.Absolute));
            tokenSource.Token.Register(delegate
            {
                _webRequest.Abort();
            });

            _webRequest.Method = "POST";
            _webRequest.ContentType = "application/octet-stream";

            Stream stream = await _webRequest.GetRequestStreamAsync();
            using (BinaryWriter requestWriter = new BinaryWriter(stream))
            {
                _request.Write(requestWriter);
            }
            WebResponse webResponse = await _webRequest.GetResponseAsync();
            BinaryResponse response;
            using (BinaryReader responseReader = new BinaryReader(webResponse.GetResponseStream()))
            {
                response = BinaryResponse.Read(responseReader);
            }
            return response;
        }
    }
}
