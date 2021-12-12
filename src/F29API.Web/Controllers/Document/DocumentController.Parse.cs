using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using F29API.Web.Models;

namespace F29API.Web.Controllers
{
    partial class DocumentController
    {
        [HttpPut]
        [RequestSizeLimit(64000000)]
        public async Task<IActionResult> Parse([FromQuery] ParseParams parms)
        {
            try
            {
                using (var contentStream = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(contentStream);

                    contentStream.Position = 0;
                    string contentType = await GetContentType(contentStream);

                    contentStream.Position = 0;
                    switch (contentType)
                    {
                        case "text/plain":
                            return await ParseDocument(parms, contentStream);
                        case "application/pdf":
                            return await ParsePdf(parms, contentStream);
                        default:
                            if (contentType.StartsWith("image/"))
                            {
                                return await ParseImage(parms, contentStream);
                            }
                            return await ParseGeneric(parms, contentStream);
                    }
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: 400);
            }
        }

        private async Task<IActionResult> ParsePdf(ParseParams parms, Stream contentStream)
        {
            string lan = GetTikaLanguage(parms.Language);

            switch (parms.Strategy)
            {
                case OcrStrategy.Auto:
                    return await ParsePdfAuto(parms, contentStream);
                case OcrStrategy.OcrInline:
                    return await ParseDocument(parms, contentStream, ("X-Tika-PDFextractInlineImages", "true"), ("X-Tika-OCRLanguage", lan));
                case OcrStrategy.OcrOnly:
                    return await ParseDocument(parms, contentStream, ("X-Tika-PDFOcrStrategy", "ocr_only"), ("X-Tika-OCRLanguage", lan));
                case OcrStrategy.None:
                default:
                    return await ParseDocument(parms, contentStream);
            }
        }

        private async Task<IActionResult> ParsePdfAuto(ParseParams parms, Stream contentStream)
        {
            var request = PUTRequest("tika", contentStream);
            (var content, var status) = await SendRequest(request);
            if (status == HttpStatusCode.OK)
            {
                if (content.Trim().Length > 0)
                {
                    string txt = CleanContent(content);
                    return Ok(ParseResponse.Ok(parms.Strategy, txt));
                }
                return Ok(ParseResponse.RequiresOcr());
            }
            return Problem(content, statusCode: (int)status);
        }

        private async Task<IActionResult> ParseImage(ParseParams parms, Stream contentStream)
        {
            string lan = GetTikaLanguage(parms.Language);

            if (parms.Strategy == OcrStrategy.Auto)
            {
                return Ok(ParseResponse.RequiresOcr());
            }
            return await ParseDocument(parms, contentStream, ("X-Tika-OCRLanguage", lan));
        }

        private async Task<IActionResult> ParseGeneric(ParseParams parms, Stream contentStream)
        {
            string lan = GetTikaLanguage(parms.Language);

            switch (parms.Strategy)
            {
                case OcrStrategy.OcrInline:
                    return await ParseDocument(parms, contentStream, ("X-Tika-OCRLanguage", lan));
                case OcrStrategy.OcrOnly:
                    return await ParseDocument(parms, contentStream, ("X-Tika-OCRLanguage", lan));
                case OcrStrategy.Auto:
                case OcrStrategy.None:
                default:
                    return await ParseDocument(parms, contentStream);
            }
        }

        private async Task<IActionResult> ParseDocument(ParseParams parms, Stream contentStream, params (string, string)[] headers)
        {
            var request = PUTRequest("tika", contentStream, headers);
            (var content, var status) = await SendRequest(request, parms.Timeout);
            if (status == HttpStatusCode.OK)
            {
                string txt = CleanContent(content);
                return Ok(ParseResponse.Ok(parms.Strategy, txt));
            }
            return Problem(content, statusCode: (int)status);
        }

        private string CleanContent(string str)
        {
            str = str.Replace("\n\n", "[N]");
            str = str.Replace("-\n", "");
            str = str.Replace("\n", " ");
            str = str.Replace("[N]", "\n");
            return str;
        }

        #region GetTikaLanguage
        private static string GetTikaLanguage(string lan)
        {
            string lang = $"{lan?.ToLower()}";
            switch (lang)
            {
                case "es":
                    lang = "spa";
                    break;
                case "fr":
                    lang = "fra";
                    break;
                case "de":
                    lang = "deu";
                    break;
                case "nl":
                    lang = "nld";
                    break;
                default:
                    lang = "eng";
                    break;
            }

            return lang;
        }
        #endregion
    }
}
