using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace FakeHttpServer
{
    public class MockHttpReponse
    {
        internal Dictionary<string, string> _headers = new Dictionary<string, string>();

        internal HttpStatusCode HttpResponseStatusCode { get; set; } = HttpStatusCode.OK;
        internal string HttpResponseContentType { get; set; }
        internal Stream HttpResponseFile { get; set; }
        internal object Data { get; set; }
        internal bool Handled { get; set; }

        internal static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        private readonly ResponseOptions _options;

        internal MockHttpReponse(ResponseOptions options)
        {
            _options = options;
        }

        public MockHttpReponse Ok(object o = null)
        {
            Data = o;

            return this;
        }

        public MockHttpReponse File(byte[] file)
        {
            HttpResponseFile = new MemoryStream(file);

            return this;
        }

        public MockHttpReponse File(string localFilePath)
        {
            HttpResponseFile = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return this;
        }

        public MockHttpReponse Json(object o)
        {
            Data = o;
            HttpResponseContentType = "application/json";

            return this;
        }

        public MockHttpReponse View(string html)
        {
            Data = html;
            HttpResponseContentType = "text/html";

            return this;
        }

        public MockHttpReponse Xml(object o)
        {
            Data = o;
            HttpResponseContentType = "application/xml";

            return this;
        }

        public MockHttpReponse Content(string content)
        {
            HttpResponseContentType = "text/plain";
            Data = content;

            return this;
        }

        public MockHttpReponse StatusCode(int statusCode)
        {
            HttpResponseStatusCode = (HttpStatusCode)statusCode;

            return this;
        }

        public MockHttpReponse StatusCode(int statusCode, object obj)
        {
            HttpResponseStatusCode = (HttpStatusCode)statusCode;
            Data = obj;

            return this;
        }

        public void AddHeader(string key, string value)
        {
            if (!_headers.ContainsKey(key))
            {
                _headers.Add(key, value);
            }
        }

        private HttpContent XmlContent()
        {
            if (Data == null)
            {
                return new StringContent("", Encoding.UTF8, "application/xml");
            }

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(Data.GetType());
            serializer.Serialize(new StringWriter(sb), Data);

            return new StringContent(sb.ToString(), Encoding.UTF8, "application/xml");
        }

        private HttpContent FileContent()
        {
            return new StreamContent(HttpResponseFile);
        }

        private HttpContent JsonContent()
        {
            return new StringContent(JsonSerializer.Serialize(Data, _options.JsonOptions ?? DefaultJsonOptions), Encoding.UTF8, HttpResponseContentType);
        }

        private HttpContent TextContent()
        {
            return new StringContent($"{Data}", Encoding.UTF8, "text/plain");
        }

        private HttpContent HtmlContent()
        {
            return new StringContent($"{Data}", Encoding.UTF8, "text/html");
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
                        acceptType = "application/json";
                    }

                    switch (acceptType)
                    {
                        case "text/html":
                            content = HtmlContent();
                            break;
                        case "application/json":
                            content = JsonContent();
                            break;
                        case "application/xml":
                            content = XmlContent();
                            break;
                        case "text/plain":
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
                Content = content,
                StatusCode = HttpResponseStatusCode,
                Version = req.Version,
                RequestMessage = req
            };

            foreach (var header in _headers)
            {
                httpResponseMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return httpResponseMessage;
        }
    }
}
