using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;

namespace F29API.Web.Services
{
    public class KeywordsService
    {
        const string SUBSCRIPTION_KEY = "<SUBSCRIPTION_KEY>";

        const string KEYWORDS = "keywords";
        const string KEYWORD_SEPARATOR = " ";

        public KeywordsService(string subscriptionKey = SUBSCRIPTION_KEY)
        {
            TextAnalyticsService = new TextAnalyticsService(subscriptionKey);
        }

        TextAnalyticsService TextAnalyticsService { get; }

        public void SetKeywords(CreateKbDTO updateKb)
        {
            foreach (var qna in updateKb.QnaList)
            {
                Process(qna);
            }
        }
        public void SetKeywords(UpdateKbOperationDTO updateKb)
        {
            var addItems = updateKb?.Add?.QnaList;
            if (addItems != null)
            {
                foreach (var qna in addItems)
                {
                    Process(qna);
                }
            }
            var updateItems = updateKb?.Update?.QnaList;
            if (updateItems != null)
            {
                foreach (var qna in updateItems)
                {
                    Process(qna);
                }
            }
        }

        // Set keywords in query
        public void SetKeywords(QueryDTO query)
        {
            var text = query.Question;
            var language = TextAnalyticsService.DetectLanguage(text);
            var keywords = TextAnalyticsService.GetKeyPhrases(text, language);
            if (keywords.Count > 0)
            {
                // Set keywords only if any
                query.Question = String.Join(" ", keywords);
            }
        }

        private void Process(QnADTO qna)
        {
            var text = qna.Answer;
            var language = TextAnalyticsService.DetectLanguage(text);
            var keywords = TextAnalyticsService.GetKeyPhrases(text, language);
            SetKeywords(qna, keywords);
        }
        private void Process(UpdateQnaDTO qna)
        {
            var text = qna.Answer;
            var language = TextAnalyticsService.DetectLanguage(text);
            var keywords = TextAnalyticsService.GetKeyPhrases(text, language);
            SetKeywords(qna, keywords);
        }

        private static void SetKeywords(QnADTO qna, IList<string> keywords)
        {
            var metadata = qna.Metadata.Where(r => r.Name != KEYWORDS).ToList();
            metadata.Add(new MetadataDTO
            {
                Name = KEYWORDS,
                // Disable keywords
                //Value = JoinKeywords(keywords)
                Value = "None"
            });
            qna.Metadata = metadata;
        }

        private static void SetKeywords(UpdateQnaDTO qna, IList<string> keywords)
        {
            var metadata = qna?.Metadata?.Add?.Where(r => r.Name != KEYWORDS).ToList();
            if (metadata != null)
            {
                metadata.Add(new MetadataDTO
                {
                    Name = KEYWORDS,
                    // Disable keywords
                    //Value = JoinKeywords(keywords)
                    Value = "None"
                });
                qna.Metadata.Add = metadata;
            }
        }

        static private string JoinKeywords(IList<string> keywords)
        {
            int index = 1;
            string text = String.Join(KEYWORD_SEPARATOR, keywords);
            while (text.Length > 500)
            {
                text = String.Join(KEYWORD_SEPARATOR, keywords.Skip(index++));
            }
            System.Diagnostics.Debug.WriteLine(text.Length);
            return text;
        }
    }
}
