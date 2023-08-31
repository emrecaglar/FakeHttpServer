using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace FakeHttpServer
{
    internal class ResponseOptions
    {
        public JsonSerializerOptions JsonOptions { get; set; }
    }
}
