using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace HttpFaker.MockHttpClient
{
    public static class MockHttpRequestExtensions
    {
        public static bool TryGetQueryStringValues(this MockHttpRequest req, string key, out StringValues values)
        {
            if (req.QueryString is null)
            {
                values = StringValues.Empty;
                return false;
            }

            var qs = req.QueryString.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (qs.Key == null)
            {
                values = StringValues.Empty;
                return false;
            }

            values = qs.Value;
            return true;
        }
    }
}
