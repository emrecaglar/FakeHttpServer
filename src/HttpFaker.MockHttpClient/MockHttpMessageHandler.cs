using HttpFaker.MockHttpClient.Binding;
using HttpFaker.MockHttpClient.Internal;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpFaker.MockHttpClient
{
    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly RouteCollection _routes;
        private readonly MockHttpRequestBinderFactory _binderFactory;
        private readonly IRouteMatcher _routeMatcher;

        public MockHttpMessageHandler(RouteCollection routes, MockHttpRequestBinderFactory binderFactory, IRouteMatcher routeMatcher)
        {
            _routes = routes;
            _binderFactory = binderFactory;
            _routeMatcher = routeMatcher;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var route = _routeMatcher.Match(_routes, request);

            if (route == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var mockHttpRequest = new MockHttpRequest
            {
                Uri = request.RequestUri,
                Headers = request.Headers,
                Version = request.Version,
                Route = request.RequestUri.Segments.Skip(1).Select(x => x.Trim('/')).ToArray(),
                QueryString = RequestPayload.ParseOrEmpty(request.RequestUri.Query),
                AllowedHttpMethods = _routes.AllowedHttpMethods(request.RequestUri.AbsolutePath)
            };

            var binder = _binderFactory.CreateBinder(request.Content?.Headers?.ContentType?.MediaType);

            await binder.BindAsync(request, mockHttpRequest);


            var mockHttpResponse = new MockHttpResponse(route.Options);

            foreach (var middlewareHandler in _routes.Middlewares)
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

            if (mockHttpResponse.ResponseIsStarted)
            {
                return new HttpResponseMessage(mockHttpResponse.HttpResponseStatusCode);
            }

            var responseMessage = route.Handler(mockHttpRequest, mockHttpResponse);

            var httpResponseMessage = responseMessage.Convert(request);

            return httpResponseMessage;
        }
    }
}
