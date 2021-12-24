using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FakeHttpServer
{
    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly List<RequestHandler> _routes;

        public MockHttpMessageHandler(List<RequestHandler> routes)
        {
            _routes = routes;
        }

        private Dictionary<string, string[]> ParseFormData(string content)
        {
            var amp = new[] { '&' };
            var eq = new[] { '=' };

            var model = new Dictionary<string, string[]>();

            if (content == null || string.IsNullOrWhiteSpace(content))
            {
                return model;
            }

            var contentParts = content.TrimStart('?').Split(amp);

            contentParts.Select(x =>
            {
                var parts = x.Split(eq);

                return new { Key = parts[0], Value = parts[1] };
            }).GroupBy(x => x.Key)
            .ToList()
            .ForEach(x =>
            {
                model.Add(x.Key, x.Select(s => WebUtility.UrlDecode(s.Value)).ToArray());
            });

            return model;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var route = _routes.Where(x => x.Method == request.Method && Regex.IsMatch(request.RequestUri.AbsolutePath.Trim('/'), x.Route.Trim('/') + "$")).FirstOrDefault();

            string content = null;

            if (request.Content != null)
            {
                content = await request.Content.ReadAsStringAsync();
            }

            var mockHttpRequest = new MockHttpRequest
            {
                Uri = request.RequestUri,
                Headers = request.Headers.ToDictionary(x => x.Key, x => string.Join("; ", x.Value)),
                Content = content,
                Version = request.Version,
                QueryString = ParseFormData(request.RequestUri.Query),
                Form = request.Content?.Headers.ContentType.MediaType == "application/x-www-form-urlencoded"
                            ? ParseFormData(content)
                            : new Dictionary<string, string[]>(),
                Route = request.RequestUri.Segments.Skip(1).Select(x => x.Trim('/')).ToArray(),



                Body = request.Content?.Headers.ContentType.MediaType == "application/json"
                            ? JsonConvert.DeserializeObject<dynamic>(content ?? "")
                            : null,


                AllowedHttpMethods = _routes.Where(x => x.Route == request.RequestUri.AbsolutePath)
                                                   .Select(x => x.Method)
                                                   .Distinct()
                                                   .ToArray()
            };

            var mockHttpResponse = new MockHttpReponse();

            var middlewareHandlers = _routes.Where(x => x.Middleware != null).ToArray();

            foreach (var middlewareHandler in middlewareHandlers)
            {
                bool goon = false;
                
                middlewareHandler.Middleware(mockHttpRequest, mockHttpResponse, () => 
                {
                    goon = true;
                });

                if (!goon)
                {
                    break;
                }
            }

            if (route == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var responseMessage = route.Handler(mockHttpRequest, mockHttpResponse);

            var httpResponseMessage = responseMessage.Convert(request);

            return httpResponseMessage;
        }
    }
}
