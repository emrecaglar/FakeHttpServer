using System;
using System.Collections.Generic;
using System.Net.Http;

namespace FakeHttpServer
{
    public class MockHttpRequest
    {
        public Uri Uri { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public string Content { get; set; }

        public Version Version { get; set; }

        public Dictionary<string, string[]> QueryString { get; set; }

        public string[] Route { get; set; }

        public Dictionary<string, string[]> Form { get; set; }

        public dynamic Body { get; set; }

        internal HttpMethod[] AllowedHttpMethods { get; set; }
    }
}
