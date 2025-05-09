using HttpFaker.Abstaction;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Serialization;

namespace HttpFaker.MockHttpClient
{
    public class MockHttpResponse
    {
        internal bool ResponseIsStarted { get; private set; }

        internal Dictionary<string, StringValues> _headers { get; set; } = new Dictionary<string, StringValues>();

        internal HttpStatusCode HttpResponseStatusCode { get; set; } = HttpStatusCode.OK;

        internal string HttpResponseContentType { get; set; }

        internal Stream HttpResponseFile { get; set; }

        internal object Data { get; set; }

        internal bool Handled { get; set; }

        private readonly FakeHttpClientOptions _options;

        internal MockHttpResponse(FakeHttpClientOptions options)
        {
            _options = options;
        }

        public MockHttpResponse Ok(object o = null)
        {
            Data = o;
            ResponseIsStarted = true;

            return this;
        }

        public MockHttpResponse File(byte[] file)
        {
            HttpResponseFile = new MemoryStream(file);
            ResponseIsStarted = true;

            return this;
        }

        public MockHttpResponse File(string localFilePath)
        {
            HttpResponseFile = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            ResponseIsStarted = true;

            return this;
        }

        public MockHttpResponse Json(object o)
        {
            Data = o;
            HttpResponseContentType = "application/json";
            ResponseIsStarted = true;

            return this;
        }

        public MockHttpResponse View(string html)
        {
            Data = html;
            HttpResponseContentType = "text/html";
            ResponseIsStarted = true;

            return this;
        }

        public MockHttpResponse Xml(object o)
        {
            Data = o;
            HttpResponseContentType = "application/xml";
            ResponseIsStarted = true;

            return this;
        }

        public MockHttpResponse Content(string content)
        {
            HttpResponseContentType = "text/plain";
            Data = content;
            ResponseIsStarted = true;

            return this;
        }

        public MockHttpResponse StatusCode(int statusCode)
        {
            HttpResponseStatusCode = (HttpStatusCode)statusCode;
            ResponseIsStarted = true;

            return this;
        }

        public MockHttpResponse StatusCode(int statusCode, object obj)
        {
            HttpResponseStatusCode = (HttpStatusCode)statusCode;
            Data = obj;
            ResponseIsStarted = true;

            return this;
        }

        public MockHttpResponse NotFound(object obj = null)
        {
            HttpResponseStatusCode = HttpStatusCode.NotFound;
            Data = obj;
            ResponseIsStarted = true;

            return this;
        }

        public bool TryAddHeader(string key, params string[] values)
        {
            if (!_headers.ContainsKey(key))
            {
                _headers.Add(key, new StringValues(values));

                return true;
            }

            return false;
        }

        private HttpContent XmlContent()
        {
            if (IsStringEmptyOrNull(Data))
            {
                return new StringContent("", Encoding.UTF8, MimeTypes.ApplicationXml);
            }

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(Data.GetType());
            serializer.Serialize(new StringWriter(sb), Data ?? string.Empty);

            return new StringContent(sb.ToString(), Encoding.UTF8, MimeTypes.ApplicationXml);
        }

        private static bool IsStringEmptyOrNull(object o) => o == null || (o is string s && string.IsNullOrWhiteSpace(s));
        

        private HttpContent FileContent()
        {
            return new StreamContent(HttpResponseFile);
        }

        private HttpContent JsonContent()
        {
            return new StringContent(IsStringEmptyOrNull(Data) ? string.Empty : _options.JsonProvider.Serialize(Data) ?? string.Empty, Encoding.UTF8, HttpResponseContentType);
        }

        private HttpContent TextContent()
        {
            return new StringContent($"{Data ?? string.Empty}", Encoding.UTF8, MimeTypes.TextPlain);
        }

        private HttpContent HtmlContent()
        {
            return new StringContent($"{Data ?? string.Empty}", Encoding.UTF8, MimeTypes.TextHtml);
        }

        internal HttpResponseMessage Convert(HttpRequestMessage req)
        {
            HttpContent content = null;

            if (req.Method != HttpMethod.Head && req.Method != HttpMethod.Options)
            {
                if (HttpResponseFile == null)
                {
                    string acceptType = HttpResponseContentType ?? req.Headers.Accept.ToString();

                    if (string.IsNullOrWhiteSpace(acceptType))
                    {
                        acceptType = MimeTypes.ApplicationJson;
                    }

                    switch (acceptType)
                    {
                        case MimeTypes.TextHtml:
                            content = HtmlContent();
                            break;
                        case MimeTypes.ApplicationJson:
                            content = JsonContent();
                            break;
                        case MimeTypes.ApplicationXml:
                            content = XmlContent();
                            break;
                        case MimeTypes.TextPlain:
                            content = TextContent();
                            break;
                    }
                }
                else
                {
                    content = FileContent();
                }
            }

            var httpResponseMessage = new HttpResponseMessage
            {
                Content = content ?? new StringContent(string.Empty),
                StatusCode = HttpResponseStatusCode,
                Version = req.Version,
                RequestMessage = req
            };

            SetResponseHeaders(httpResponseMessage, _options.DefaultResponseHeaders);

            SetResponseHeaders(httpResponseMessage, _headers);

            return httpResponseMessage;
        }

        private static void SetResponseHeaders(HttpResponseMessage httpResponseMessage, Dictionary<string, StringValues> headers)
        {
            foreach (var header in headers)
            {
                if (httpResponseMessage.Headers.Contains(header.Key))
                {
                    httpResponseMessage.Headers.Remove(header.Value);
                }

                httpResponseMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.AsEnumerable());
            }
        }
    }
}
