using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace HttpFaker.MockHttpClient.Extensions
{
    public static class SringEnumerableExtensions
    {
        public static StringValues ToStringValues(this IEnumerable<string> values)
        {
            return new StringValues(values.ToArray());
        }
    }
}