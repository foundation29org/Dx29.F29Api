using System;
using System.ComponentModel;

namespace F29API.Web.Models
{
    public enum OcrStrategy
    {
        Auto,
        OcrInline,
        OcrOnly,
        None
    }

    public class ParseParams
    {
        [DefaultValue(OcrStrategy.Auto)]
        public OcrStrategy Strategy { get; set; } = OcrStrategy.Auto;

        [DefaultValue("en")]
        public string Language { get; set; }

        [DefaultValue("text/plain")]
        public string ContentType { get; set; } = "text/plain";

        [DefaultValue(120)]
        public int Timeout { get; set; } = 120;
    }
}
