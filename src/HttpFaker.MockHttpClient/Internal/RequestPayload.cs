using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace HttpFaker.MockHttpClient.Internal
{
    internal struct PayloadInput
    {
        public PayloadInput(string key, string value) : this()
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public string Value { get; set; }

        public static StringValues Convert(IGrouping<string, PayloadInput> input)
        {
            return new StringValues(input.Select(x => WebUtility.UrlDecode(x.Value)).ToArray());
        }
    }

    internal class RequestPayload
    {
        private static readonly char[] amp = new[] { '&' };
        private static readonly char[] eq = new[] { '=' };

        public readonly static IEnumerable<KeyValuePair<string, StringValues>> EmptyContent = Enumerable.Empty<KeyValuePair<string, StringValues>>();

        public static bool TryParse(string payload, out IEnumerable<KeyValuePair<string, StringValues>> content)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                content = Enumerable.Empty<KeyValuePair<string, StringValues>>();
                return false;
            }

            try
            {
                content = Parse(payload);
                return true;
            }
            catch
            {
                content = Enumerable.Empty<KeyValuePair<string, StringValues>>();
                return false;
            }
        }

        public static IEnumerable<KeyValuePair<string, StringValues>> Parse(string payload)
        {
            var items = payload.TrimStart('?').Split(amp, StringSplitOptions.RemoveEmptyEntries);

            var inputs = items.Select(x =>
            {
                var token = x.Split(eq);

                var key = token[0];

                if (token.Length > 1)
                {

                    var val = token[1];

                    return new PayloadInput(key, val);
                }

                return new PayloadInput(key, string.Empty);
            });

            return inputs.GroupBy(x => x.Key)
                         .Select(x =>
                             new KeyValuePair<string, StringValues>(x.Key, PayloadInput.Convert(x))
                         );
        }

        public static IEnumerable<KeyValuePair<string, StringValues>> ParseOrEmpty(string payload)
        {
            if (TryParse(payload, out var content))
            {
                return content;
            }

            return EmptyContent;
        }
    }
}
