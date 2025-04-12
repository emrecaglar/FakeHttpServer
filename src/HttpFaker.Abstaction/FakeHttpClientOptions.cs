using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace HttpFaker.Abstaction
{
    public class FakeHttpClientOptions
    {
        public IJsonSeralizationProvider JsonProvider { get; set; }

        public Dictionary<string, StringValues> DefaultResponseHeaders { get; set; } = new Dictionary<string, StringValues>();
    }
}
