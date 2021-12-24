using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FakeHttpServer
{
    public delegate void MockMiddlewareHandler(MockHttpRequest req, MockHttpReponse res, Action next);

    public class RequestHandler
    {
        internal HttpMethod Method { get; set; }
        internal string Route { get; set; }
        internal Func<MockHttpRequest, MockHttpReponse, MockHttpReponse> Handler { get; set; }

        internal MockMiddlewareHandler Middleware { get; set; }

        public void UseMiddleware(MockMiddlewareHandler handler)
        {
            Middleware = handler;
        }


        private void SET(
            HttpMethod method,
            string route,
            Func<MockHttpRequest, MockHttpReponse, MockHttpReponse> handler)
        {
            Method = method;
            Route = route;
            Handler = handler;
        }

        public void TRACE(string route)
        {
            SET(HttpMethod.Trace, route, (req, res) =>
            {
                res.HttpResponseStatusCode = HttpStatusCode.OK;

                foreach (var header in req.Headers)
                {
                    res.AddHeader(header.Key, string.Join("; ", header.Value));
                }

                res.Data = req.Content;
                res.HttpResponseContentType = $"{req.Headers["Content-Type"]}";

                return res;
            });
        }

        public void OPTIONS(string route)
        {
            SET(HttpMethod.Options, route, (req, res) =>
            {
                res.AddHeader("Access-Control-Allow-Methods", string.Join(", ", req.AllowedHttpMethods.Select(x => x.ToString().ToUpper())));
                res.HttpResponseStatusCode = HttpStatusCode.NoContent;

                return res;
            });
        }

        public void HEAD(string route, Func<MockHttpRequest, MockHttpReponse, MockHttpReponse> handler)
        {
            SET(HttpMethod.Head, route, handler);
        }

        public void DELETE(string route, Func<MockHttpRequest, MockHttpReponse, MockHttpReponse> handler)
        {
            SET(HttpMethod.Delete, route, handler);
        }

        public void GET(string route, Func<MockHttpRequest, MockHttpReponse, MockHttpReponse> handler)
        {
            SET(HttpMethod.Get, route, handler);
        }

        public void POST(string route, Func<MockHttpRequest, MockHttpReponse, MockHttpReponse> handler)
        {
            SET(HttpMethod.Post, route, handler);
        }

        public void PUT(string route, Func<MockHttpRequest, MockHttpReponse, MockHttpReponse> handler)
        {
            SET(HttpMethod.Put, route, handler);
        }
    }
}
