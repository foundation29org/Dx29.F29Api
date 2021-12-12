using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;

using F29API.Web.Services;

using Newtonsoft.Json;
using System.Collections.Generic;

namespace F29API.Web.Controllers
{
    [ApiController]
    public class KnowledgeQueryController : ControllerBase
    {
        const string ENDPOINT = "https://{0}.azurewebsites.net";

        private HttpService _httpService = null;
        private KeywordsService _keywordsService = null;

        public KnowledgeQueryController(HttpService httpService, KeywordsService keywordsService)
        {
            _httpService = httpService;
            _keywordsService = keywordsService;
        }

        [HttpPost("api/generateanswer/{domain}/{kbid}")]
        public async Task<IActionResult> GenerateAnswer(string domain, string kbid, string category = null, string environment = "Prod", [FromBody] QueryDTO query = null)
        {
            if (String.IsNullOrEmpty(category))
            {
                if (query == null) return BadRequest("QueryDTO not found in query.");
                _keywordsService.SetKeywords(query);
                return await GenerateAnswerByQuery(domain, kbid, query);
            }
            return await GenerateAnswerByCategory(kbid, environment, category);
        }

        private async Task<IActionResult> GenerateAnswerByQuery(string domain, string kbid, QueryDTO query)
        {
            var url = String.Format(ENDPOINT, domain);
            var svc = new KBRuntimeService(url, GetAuthorization());
            return Ok(await svc.GenerateAnswerAsync(kbid, query));
        }

        private async Task<IActionResult> GenerateAnswerByCategory(string kbid, string environment, string category)
        {
            var request = _httpService.GETRequest($"knowledgebases/{kbid}/{environment}/qna", GetSubscriptionKeyHeader());
            (var content, var status) = await _httpService.SendRequest(request);
            if (status == HttpStatusCode.OK)
            {
                var docs = JsonConvert.DeserializeObject<QnADocumentsDTO>(content);
                var items = docs.QnaDocuments.Where(r => MatchCategory(r.Metadata, category));
                var result = CreateSearchResultsFromDocuments(items);
                return Ok(result);
            }
            throw new ApplicationException(content);
        }

        private static bool MatchCategory(IList<MetadataDTO> metadata, string category)
        {
            System.Diagnostics.Debug.WriteLine(metadata[0].Name);
            if (metadata.Count > 1)
            {
                System.Diagnostics.Debug.WriteLine(metadata[1].Name);
            }
            var cat = metadata.Where(r => r.Name.Equals("category", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (cat != null)
            {
                var values = cat.Value.Split(';');
                return values.Any(r => r.Trim().Equals(category, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }

        static private QnASearchResultList CreateSearchResultsFromDocuments(IEnumerable<QnADTO> documents)
        {
            var searchResults = new QnASearchResultList(new List<QnASearchResult>());
            foreach (var doc in documents)
            {
                var searchResult = new QnASearchResult(doc.Questions, doc.Answer, 1.0, doc.Id, doc.Source, doc.Metadata);
                searchResults.Answers.Add(searchResult);
            }
            return searchResults;
        }

        private string GetAuthorization()
        {
            var key = Request.Headers["Authorization"];
            return key.ToString().Replace("EndpointKey", "").Trim();
        }

        private (string, string)[] GetSubscriptionKeyHeader()
        {
            var key = Request.Headers["Ocp-Apim-Subscription-Key"];
            return new (string, string)[] { ("Ocp-Apim-Subscription-Key", key) };
        }
    }
}
