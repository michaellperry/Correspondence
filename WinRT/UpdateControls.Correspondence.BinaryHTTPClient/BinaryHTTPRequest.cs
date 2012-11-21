using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.BinaryHTTPClient
{
    public static class BinaryHTTPRequest
    {
        public static async Task<BinaryResponse> SendAsync(
            HTTPConfiguration configuration,
            BinaryRequest request)
        {
            var _request = request;

            var _webRequest = WebRequest.Create(new Uri(configuration.Endpoint, UriKind.Absolute));
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
