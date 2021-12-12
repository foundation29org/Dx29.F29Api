using System;
using System.ComponentModel;

namespace F29API.Web.Models
{
    public enum ParseStatus
    {
        Ok,
        RequireOcr,
        Error
    }

    public class ParseResponse
    {
        public ParseStatus Status { get; set; }
        public OcrStrategy Strategy { get; set; }
        public string Content { get; set; }
        public string Message { get; set; }

        static public ParseResponse Ok(OcrStrategy strategy, string content)
        {
            return new ParseResponse
            {
                Status = ParseStatus.Ok,
                Strategy = strategy,
                Content = content
            };
        }
        static public ParseResponse RequiresOcr()
        {
            return new ParseResponse
            {
                Status = ParseStatus.RequireOcr,
                Strategy = OcrStrategy.Auto,
                Message = "This document requires OCR parsing. Please, select OCR option."
            };
        }
        static public ParseResponse Error(OcrStrategy strategy, string message)
        {
            return new ParseResponse
            {
                Status = ParseStatus.Error,
                Strategy = strategy,
                Message = message
            };
        }
    }
}
