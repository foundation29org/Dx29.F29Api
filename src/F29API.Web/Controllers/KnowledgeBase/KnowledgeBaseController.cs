using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;

using F29API.Web.Services;

namespace F29API.Web.Controllers
{
    [ApiController]
    public partial class KnowledgeBaseController : ControllerBase
    {
        const string ENDPOINT = "https://westus.api.cognitive.microsoft.com/qnamaker/v4.0/knowledgebases";

        private HttpService _httpService = null;
        private KeywordsService _keywordsService = null;

        public KnowledgeBaseController(HttpService httpService, KeywordsService keywordsService)
        {
            _httpService = httpService;
            _keywordsService = keywordsService;
        }

        [HttpPost("api/knowledgebases/create")]
        public async Task<string> Create([FromBody] CreateKbDTO createKb)
        {
            _keywordsService.SetKeywords(createKb);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(createKb);
            var bytes = Encoding.UTF8.GetBytes(json);
            using (var body = new ByteArrayContent(bytes))
            {
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var request = _httpService.POSTRequest($"knowledgebases/create", body, GetHeader());
                (var content, var status) = await _httpService.SendRequest(request);
                if (status == HttpStatusCode.OK || status == HttpStatusCode.Accepted)
                {
                    return content;
                }
                throw new ApplicationException(content);
            }
        }

        [HttpPatch("api/knowledgebases/{kbId}")]
        public async Task<string> Update(string kbId, [FromBody] UpdateKbOperationDTO updateKb)
        {
            _keywordsService.SetKeywords(updateKb);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(updateKb);
            var bytes = Encoding.UTF8.GetBytes(json);
            using (var body = new ByteArrayContent(bytes))
            {
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var request = _httpService.PATCHRequest($"knowledgebases/{kbId}", body, GetHeader());
                (var content, var status) = await _httpService.SendRequest(request);
                if (status == HttpStatusCode.OK || status == HttpStatusCode.Accepted)
                {
                    return content;
                }
                throw new ApplicationException(content);
            }
        }

        [HttpGet("api/knowledgebases/{kbId}")]
        public async Task<string> Details(string kbId)
        {
            var request = _httpService.GETRequest($"knowledgebases/{kbId}", GetHeader());
            (var content, var status) = await _httpService.SendRequest(request);
            if (status == HttpStatusCode.OK)
            {
                return content;
            }
            throw new ApplicationException(content);
        }

        [HttpPost("api/knowledgebases/{kbId}")]
        public async Task<string> Publish(string kbId)
        {
            using (var body = new StringContent("{body}"))
            {
                var request = _httpService.POSTRequest($"knowledgebases/{kbId}", body, GetHeader());
                (var content, var status) = await _httpService.SendRequest(request);
                if (status == HttpStatusCode.OK || status == HttpStatusCode.NoContent)
                {
                    return content;
                }
                throw new ApplicationException(content);
            }
        }

        [HttpGet("api/knowledgebases/{kbId}/{environment}/qna")]
        public async Task<string> Download(string kbId, string environment)
        {
            var request = _httpService.GETRequest($"knowledgebases/{kbId}/{environment}/qna", GetHeader());
            (var content, var status) = await _httpService.SendRequest(request);
            if (status == HttpStatusCode.OK)
            {
                return content;
            }
            throw new ApplicationException(content);
        }

        [HttpGet("api/operations/{operationId}")]
        public async Task<string> Operations(string operationId)
        {
            var request = _httpService.GETRequest($"operations/{operationId}", GetHeader());
            (var content, var status) = await _httpService.SendRequest(request);
            if (status == HttpStatusCode.OK)
            {
                return content;
            }
            throw new ApplicationException(content);
        }

        [HttpDelete("api/knowledgebases/{kbId}")]
        public async Task<string> Delete(string kbId)
        {
            using (var body = new StringContent("{body}"))
            {
                var request = _httpService.DELETERequest($"knowledgebases/{kbId}", body, GetHeader());
                (var content, var status) = await _httpService.SendRequest(request);
                if (status == HttpStatusCode.OK || status == HttpStatusCode.NoContent)
                {
                    return content;
                }
                throw new ApplicationException(content);
            }
        }

        private (string, string)[] GetHeader()
        {
            var subscriptionKey = Request.Headers["Ocp-Apim-Subscription-Key"];
            return new (string, string)[] { ("Ocp-Apim-Subscription-Key", subscriptionKey) };
        }
    }
}
