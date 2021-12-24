using System;
using System.Linq;
using System.Net.Http;

namespace FakeHttpServer
{
    public class FakeHttpServer
    {
        public HttpClient Setup(params Action<RequestHandler>[] handlers)
        {
            var requestHandlers = handlers.Select(action =>
            {
                var handler = new RequestHandler();

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
