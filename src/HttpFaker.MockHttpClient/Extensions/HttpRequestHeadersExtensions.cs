using System.Collections.Generic;

namespace System.Net.Http.Headers
{
    public static class HttpRequestHeadersExtensions
    {
        public static string GetValueAsString(this HttpRequestHeaders header, string name)
        {
            return header.GetValues(name).ToStringValues().ToString();
        }
    }
}
