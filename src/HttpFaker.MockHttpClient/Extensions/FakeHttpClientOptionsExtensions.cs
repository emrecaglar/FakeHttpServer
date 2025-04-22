using Microsoft.Extensions.Primitives;

namespace HttpFaker.Abstaction
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
