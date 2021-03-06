﻿using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Correspondence.BinaryHTTPClient
{
    public static class BinaryHTTPRequest
    {
        public static async Task<BinaryResponse> SendAsync(
            HTTPConfiguration configuration,
            BinaryRequest request)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            int timeoutSeconds = Math.Min(configuration.TimeoutSeconds, 30);
            if (timeoutSeconds > 0)
                tokenSource.CancelAfter(timeoutSeconds * 1500);
            return await InternalSendAsync(configuration, request, tokenSource);
        }

        private static async Task<BinaryResponse> InternalSendAsync(
            HTTPConfiguration configuration,
            BinaryRequest request,
            CancellationTokenSource tokenSource)
        {
            try
            {
                var webRequest = WebRequest.Create(new Uri(configuration.Endpoint, UriKind.Absolute));
                tokenSource.Token.Register(delegate
                {
                    webRequest.Abort();
                });

                webRequest.Method = "POST";
                webRequest.ContentType = "application/octet-stream";

                Stream stream = await Task.Factory.FromAsync<Stream>(webRequest.BeginGetRequestStream, webRequest.EndGetRequestStream, null);
                using (BinaryWriter requestWriter = new BinaryWriter(stream))
                {
                    request.Write(requestWriter);
                }
                WebResponse webResponse = await Task.Factory.FromAsync<WebResponse>(webRequest.BeginGetResponse, webRequest.EndGetResponse, null);
                BinaryResponse response;
                using (BinaryReader responseReader = new BinaryReader(webResponse.GetResponseStream()))
                {
                    response = BinaryResponse.Read(responseReader);
                }
                return response;
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    var webResponse = (HttpWebResponse)we.Response;
                    string httpStatus = webResponse.StatusCode + ": " + webResponse.StatusDescription;
                    try
                    {
                        using (var errorReader = new StreamReader(we.Response.GetResponseStream()))
                        {
                            string page = errorReader.ReadToEnd();
                            throw new CorrespondenceException(httpStatus + "\n" + page);
                        }
                    }
                    catch (Exception)
                    {
                        throw new CorrespondenceException(httpStatus);
                    }
                }
                else
                {
                    throw new CorrespondenceException("Error while sending HTTP request: " + we.Status);
                }
            }
        }
    }
}
