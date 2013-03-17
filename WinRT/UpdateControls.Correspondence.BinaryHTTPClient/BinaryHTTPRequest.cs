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
            int timeoutSeconds = Math.Min(configuration.TimeoutSeconds, 30);
            tokenSource.CancelAfter(timeoutSeconds * 1500);
            return await InternalSendAsync(configuration, request, tokenSource);
        }

        private static async Task<BinaryResponse> InternalSendAsync(
            HTTPConfiguration configuration,
            BinaryRequest request,
            CancellationTokenSource tokenSource)
        {
            var webRequest = WebRequest.Create(new Uri(configuration.Endpoint, UriKind.Absolute));
            tokenSource.Token.Register(delegate
            {
                webRequest.Abort();
            });

            webRequest.Method = "POST";
            webRequest.ContentType = "application/octet-stream";

            Stream stream = await webRequest.GetRequestStreamAsync();
            using (BinaryWriter requestWriter = new BinaryWriter(stream))
            {
                request.Write(requestWriter);
            }
            WebResponse webResponse = await webRequest.GetResponseAsync();
            BinaryResponse response;
            using (BinaryReader responseReader = new BinaryReader(webResponse.GetResponseStream()))
            {
                response = BinaryResponse.Read(responseReader);
            }
            return response;
        }
    }
}
