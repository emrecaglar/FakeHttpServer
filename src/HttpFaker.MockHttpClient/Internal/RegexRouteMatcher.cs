using System.Net.Http;
using System.Text.RegularExpressions;

namespace HttpFaker.MockHttpClient.Internal
{
    internal interface IRouteMatcher
    {
        RequestHandler Match(RouteCollection routes, HttpRequestMessage httpRequestMessage);
    }

    internal class RegexRouteMatcher : IRouteMatcher
    {
        public RequestHandler Match(RouteCollection routes, HttpRequestMessage httpRequestMessage)
        {
            return routes.Find(x => x.Method == httpRequestMessage.Method && Regex.IsMatch(httpRequestMessage.RequestUri.AbsolutePath.Trim('/'), x.Route.Trim('/') + "$"));
        }
    }
}
