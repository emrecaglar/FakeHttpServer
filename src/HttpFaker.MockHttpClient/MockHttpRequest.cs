using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HttpFaker.MockHttpClient
{
    public class MockHttpRequest
    {
        public Uri Uri { get; set; }

        public HttpRequestHeaders Headers { get; set; }

        public string Payload { get; set; }

        public Version Version { get; set; }

        public IEnumerable<KeyValuePair<string, StringValues>> QueryString { get; set; }

        public string[] Route { get; set; }

        public IEnumerable<KeyValuePair<string, StringValues>> Form { get; set; }

        public IReadOnlyList<byte[]> FormFiles { get; set; }

        public byte[] Body { get; set; }

        internal HttpMethod[] AllowedHttpMethods { get; set; }
    }
}
