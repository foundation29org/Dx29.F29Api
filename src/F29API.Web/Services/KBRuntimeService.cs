using System;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;

namespace F29API.Web.Services
{
    public class KBRuntimeService
    {
        const string ENDPOINT = "<QNA_ENDPOINT>";
        const string ENDPOINT_KEY = "<ENDPOINT_KEY>";

        private IQnAMakerRuntimeClient _client = null;

        public KBRuntimeService() : this(ENDPOINT, ENDPOINT_KEY) { }
        public KBRuntimeService(string endpoint, string endpointKey)
        {
            Endpoint = endpoint;
            EndpointKey = endpointKey;
        }

        public string Endpoint { get; set; }
        public string EndpointKey { get; set; }

        public IQnAMakerRuntimeClient QnARuntime => _client ?? new QnAMakerRuntimeClient(new ApiKeyServiceClientCredentials(EndpointKey)) { RuntimeEndpoint = Endpoint };

        public async Task<QnASearchResultList> GenerateAnswerAsync(string kbid, string question, double? scoreThreshold = 25.0, int? top = 10)
        {
            var query = new QueryDTO
            {
                Question = question,
                ScoreThreshold = scoreThreshold,
                Top = top
            };
            return await GenerateAnswerAsync(kbid, query);
        }

        public async Task<QnASearchResultList> GenerateAnswerAsync(string kbid, QueryDTO query)
        {
            var runtimeClient = new QnAMakerRuntimeClient(new EndpointKeyServiceClientCredentials(EndpointKey)) { RuntimeEndpoint = Endpoint };
            var result = await runtimeClient.Runtime.GenerateAnswerAsync(kbid, query);
            return result;
        }
    }
}
