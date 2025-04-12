using HttpFaker.Abstaction;
using Microsoft.Extensions.Primitives;

namespace HttpFaker.MockHttpClient.Extensions
{
    public static class FakeHttpClientOptionsExtensions
    {
        public static FakeHttpClientOptions TryAddResponseHeader(this FakeHttpClientOptions options, string name, StringValues values)
        {
            if (options.DefaultResponseHeaders.ContainsKey(name))
            {
                options.DefaultResponseHeaders[name] = values;
            }
            else
            {
                options.DefaultResponseHeaders.Add(name, values);
            }

            return options;
        }
    }
}
