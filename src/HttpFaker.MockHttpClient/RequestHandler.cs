using HttpFaker.Abstaction;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace HttpFaker.MockHttpClient
{
    public delegate void MockMiddlewareHandler(MockHttpRequest req, MockHttpResponse res, Action next);

    public class RequestHandler
    {
        internal HttpMethod Method { get; set; }
        internal string Route { get; set; }
        internal Func<MockHttpRequest, MockHttpResponse, MockHttpResponse> Handler { get; set; }

        internal MockMiddlewareHandler Middleware { get; set; }

        internal FakeHttpClientOptions Options { get; }

        internal RequestHandler(FakeHttpClientOptions options)
        {
            Options = options;
        }

        public void UseMiddleware(MockMiddlewareHandler handler)
        {
            Middleware = handler;
        }

        private void MapSet(
            HttpMethod method,
            string route,
            Func<MockHttpRequest, MockHttpResponse, MockHttpResponse> handler)
        {
            Method = method;
            Route = route;
            Handler = handler;
        }

        public void MapTrace(string route)
        {
            MapSet(HttpMethod.Trace, route, (req, res) =>
            {
                res.HttpResponseStatusCode = HttpStatusCode.OK;

                foreach (var header in req.Headers)
                {
                    res.TryAddHeader(header.Key, string.Join("; ", header.Value));
                }

                res.Data = req.Payload;
                res.HttpResponseContentType = $"{(req.Headers.TryGetValues("Content-Type", out var values) ? string.Concat(values) : string.Empty)}";

                return res;
            });
        }

        public void MapOptions(string route)
        {
            MapSet(HttpMethod.Options, route, (req, res) =>
            {
                res.TryAddHeader("Access-Control-Allow-Methods", string.Join(", ", req.AllowedHttpMethods.Select(x => x.ToString().ToUpper())));
                res.HttpResponseStatusCode = HttpStatusCode.NoContent;

                return res;
            });
        }

        public void MapHead(string route, Func<MockHttpRequest, MockHttpResponse, MockHttpResponse> handler)
        {
            MapSet(HttpMethod.Head, route, handler);
        }

        public void MapDelete(string route, Func<MockHttpRequest, MockHttpResponse, MockHttpResponse> handler)
        {
            MapSet(HttpMethod.Delete, route, handler);
        }

        public void MapGet(string route, Func<MockHttpRequest, MockHttpResponse, MockHttpResponse> handler)
        {
            MapSet(HttpMethod.Get, route, handler);
        }

        public void MapPost(string route, Func<MockHttpRequest, MockHttpResponse, MockHttpResponse> handler)
        {
            MapSet(HttpMethod.Post, route, handler);
        }

        public void MapPut(string route, Func<MockHttpRequest, MockHttpResponse, MockHttpResponse> handler)
        {
            MapSet(HttpMethod.Put, route, handler);
        }
    }
}
