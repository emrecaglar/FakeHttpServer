using Microsoft.Extensions.Primitives;
using System.Linq;

namespace System.Collections.Generic
{
    public static class SringEnumerableExtensions
    {
        public static StringValues ToStringValues(this IEnumerable<string> values)
        {
            return new StringValues(values.ToArray());
        }
    }
}