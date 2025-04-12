using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace HttpFaker.MockHttpClient.Internal
{
    public class RouteCollection : List<RequestHandler>
    {
        public RouteCollection()
        {
            
        }

        public RouteCollection(IEnumerable<RequestHandler> routes)
        {
            this.AddRange(routes);
        }

        public HttpMethod[] AllowedHttpMethods(string path) => this.Where(x => x.Route == path)
                                            .Select(x => x.Method)
                                            .Distinct()
                                            .ToArray();

        public IEnumerable<RequestHandler> Middlewares => this.Where(x => x.Middleware != null);
    }
}
