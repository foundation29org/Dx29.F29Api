using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using F29API.Web.Models;

namespace F29API.Web.Controllers
{
    partial class DocumentController
    {
        private async Task<string> GetContentType(Stream contentStream)
        {
            var request = PUTRequest("/detect/stream", contentStream);
            (var content, var status) = await SendRequest(request);
            if (status == HttpStatusCode.OK)
            {
                return content;
            }
            throw new ApplicationException(content);
        }

        private HttpRequestMessage GETRequest(string action, params (string, string)[] headers)
        {
            return CreateRequest(action, HttpMethod.Get, headers);
        }

        private HttpRequestMessage POSTRequest(string action, Stream content, params (string, string)[] headers)
        {
            return POSTRequest(action, new StreamContent(content), headers);
        }
        private HttpRequestMessage POSTRequest(string action, HttpContent content, params (string, string)[] headers)
        {
            var request = CreateRequest(action, HttpMethod.Post, headers);
            request.Content = content;
            return request;
        }

        private HttpRequestMessage PUTRequest(string action, Stream content, params (string, string)[] headers)
        {
            return PUTRequest(action, new StreamContent(content), headers);
        }
        private HttpRequestMessage PUTRequest(string action, HttpContent content, params (string, string)[] headers)
        {
            var request = CreateRequest(action, HttpMethod.Put, headers);
            request.Content = content;
            return request;
        }

        private HttpRequestMessage CreateRequest(string action, HttpMethod method, params (string, string)[] headers)
        {
            var request = new HttpRequestMessage(method, $"{BASE_URL}/{action}");
            foreach (var header in headers)
            {
                request.Headers.Add(header.Item1, header.Item2);
            }
            return request;
        }

        private async Task<(string, HttpStatusCode)> SendRequest(HttpRequestMessage request, double timeout = 120)
        {
            var client = _clientFactory.CreateClient();
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
