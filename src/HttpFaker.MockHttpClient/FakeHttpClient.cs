using HttpFaker.Abstaction;
using HttpFaker.MockHttpClient.Binding;
using HttpFaker.MockHttpClient.Extensions;
using HttpFaker.MockHttpClient.Internal;
using HttpFaker.Serilization.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HttpFaker.MockHttpClient
{
    public class FakeHttpClient
    {
        public FakeHttpClient()
        {

        }

        public FakeHttpClient(Action<FakeHttpClientOptions> config) : this()
        {
            config(Options);
        }

        public FakeHttpClientOptions Options { get; set; } = new FakeHttpClientOptions
        {
            JsonProvider = new SystemTextJsonProvider(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            }),
            Binders = new Dictionary<string, Type> 
            {
                { MimeTypes.ApplicationJson, typeof(RawBinder) },
                { MimeTypes.ApplicationXml, typeof(RawBinder) },
                { MimeTypes.ApplicationFormUrlEncoded, typeof(FormBinder) },
                { MimeTypes.MultiPartFormData, typeof(MultipartFormDataBinder) },
                { MimeTypes.MultiPartMixed, typeof(MultipartFormDataBinder) },
            }
        };

        public HttpClient Setup(params Action<RequestHandler>[] handlers)
        {
            var requestHandlers = handlers.Select(action =>
            {
                var handler = new RequestHandler(Options);

                action(handler);

                return handler;
            }).ToList();

            return new HttpClient(new MockHttpMessageHandler(new RouteCollection(requestHandlers), new MockHttpRequestBinderFactory(Options.Binders), new RegexRouteMatcher()))
            {
                BaseAddress = new Uri("http://fake.http")
            };
        }
    }
}
