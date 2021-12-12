using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;

using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

using Newtonsoft.Json;

namespace F29API.Web.Services
{
    public class TextAnalyticsService
    {
        const string ENDPOINT = "<text-analytics-endpoint>";

        public TextAnalyticsService(string subscriptionKey)
        {
            SubscriptionKey = subscriptionKey;
        }

        public string SubscriptionKey { get; }

        public string DetectLanguage(string text)
        {
            var input = new LanguageBatchInput(new LanguageInput[] { new LanguageInput("1", text) });
            var body = JsonConvert.SerializeObject(input);
            (var json, var status) = MakeRequest("languages", "?showStats=false", body: body, method: HttpMethod.Post).Result;
            var result = JsonConvert.DeserializeObject<LanguageBatchResult>(json);
            return result?.Documents.FirstOrDefault()?.DetectedLanguages?.FirstOrDefault().Iso6391Name;
        }

        public IList<string> GetKeyPhrases(string text, string language)
        {
            var doc = new
            {
                Id = 1,
                Language = language,
                Text = text
            };
            var docs = new
            {
                Documents = new object[] { doc }
            };
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(docs, Newtonsoft.Json.Formatting.Indented);
            (var json, var status) = MakeRequest("keyPhrases", "?showStats=false", body: body, method: HttpMethod.Post).Result;
            var keyPhraseResult = Newtonsoft.Json.JsonConvert.DeserializeObject<KeyPhraseBatchResult>(json);
            return keyPhraseResult.Documents[0].KeyPhrases;
        }

        private async Task<(string, HttpStatusCode)> MakeRequest(string action, string queryString = null, string body = null, HttpMethod method = null)
        {
            var client = new HttpClient();
            var byteData = Encoding.UTF8.GetBytes(body ?? "");
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
                var request = new HttpRequestMessage(method ?? HttpMethod.Get, $"{ENDPOINT}/{action}{queryString}") { Content = content };
                var response = await client.SendAsync(request);
                return (await response.Content.ReadAsStringAsync(), response.StatusCode);
            }
        }
    }
}
