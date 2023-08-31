using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace FakeHttpServer
{
    public class FakeHttpClient
    {
        private JsonSerializerOptions _jsonSerializerOptions = null;

        public FakeHttpClient AddJsonOptions(Action<JsonSerializerOptions> opt)
        {
            _jsonSerializerOptions = new JsonSerializerOptions();

            opt(_jsonSerializerOptions);

            return this;
        }

        public HttpClient Setup(params Action<RequestHandler>[] handlers)
        {
            var requestHandlers = handlers.Select(action =>
            {
                var handler = new RequestHandler(new ResponseOptions
                {
                    JsonOptions = _jsonSerializerOptions
                });

                action(handler);

                return handler;
            }).ToList();

            return new HttpClient(new MockHttpMessageHandler(requestHandlers))
            {
                BaseAddress = new Uri("http://local")
            };
        }
    }
}
