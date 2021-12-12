using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace F29API.Web.Services
{
    public class HttpService
    {
        const string ENDPOINT = "<qnamaker-endpoint>";

        public HttpService(IHttpClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
        }

        public IHttpClientFactory ClientFactory { get; }

        public HttpRequestMessage GETRequest(string action, params (string, string)[] headers)
        {
            return CreateRequest(action, HttpMethod.Get, headers);
        }

        public HttpRequestMessage POSTRequest(string action, Stream content, params (string, string)[] headers)
        {
            return POSTRequest(action, new StreamContent(content), headers);
        }
        public HttpRequestMessage POSTRequest(string action, HttpContent content, params (string, string)[] headers)
        {
            var request = CreateRequest(action, HttpMethod.Post, headers);
            request.Content = content;
            return request;
        }

        public HttpRequestMessage PUTRequest(string action, Stream content, params (string, string)[] headers)
        {
            return PUTRequest(action, new StreamContent(content), headers);
        }
        public HttpRequestMessage PUTRequest(string action, HttpContent content, params (string, string)[] headers)
        {
            var request = CreateRequest(action, HttpMethod.Put, headers);
            request.Content = content;
            return request;
        }

        public HttpRequestMessage PATCHRequest(string action, Stream content, params (string, string)[] headers)
        {
            return PATCHRequest(action, new StreamContent(content), headers);
        }
        public HttpRequestMessage PATCHRequest(string action, HttpContent content, params (string, string)[] headers)
        {
            var request = CreateRequest(action, HttpMethod.Patch, headers);
            request.Content = content;
            return request;
        }

        public HttpRequestMessage DELETERequest(string action, Stream content, params (string, string)[] headers)
        {
            return DELETERequest(action, new StreamContent(content), headers);
        }
        public HttpRequestMessage DELETERequest(string action, HttpContent content, params (string, string)[] headers)
        {
            var request = CreateRequest(action, HttpMethod.Delete, headers);
            request.Content = content;
            return request;
        }

        public HttpRequestMessage CreateRequest(string action, HttpMethod method, params (string, string)[] headers)
        {
            var request = new HttpRequestMessage(method, $"{ENDPOINT}/{action}");
            foreach (var header in headers)
            {
                request.Headers.Add(header.Item1, header.Item2);
            }
            return request;
        }

        public async Task<(string, HttpStatusCode)> SendRequest(HttpRequestMessage request, double timeout = 120)
        {
            var client = ClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(timeout);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadAsStringAsync(), response.StatusCode);
            }
            return (response.ReasonPhrase, response.StatusCode);
        }
    }
}
