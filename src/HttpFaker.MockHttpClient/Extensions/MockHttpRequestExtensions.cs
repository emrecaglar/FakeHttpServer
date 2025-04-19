using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Net.Http.Headers;

namespace HttpFaker.MockHttpClient.Extensions
{
    public static class MockHttpRequestExtensions
    {
        public static bool TryGetQueryStringValues(this MockHttpRequest req, string key, out StringValues values)
        {
            var qs = req.QueryString.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (qs.Key == null)
            {
                values = StringValues.Empty;
                return false;
            }

            values = qs.Value;
            return true;
        }

        public static string GetValueAsString(this HttpRequestHeaders header, string name)
        {
            return header.GetValues(name).ToStringValues().ToString();
        }
    }
}
