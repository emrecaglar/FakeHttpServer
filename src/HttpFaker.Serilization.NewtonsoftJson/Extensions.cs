using HttpFaker.Serilization.NewtonsoftJson;
using Newtonsoft.Json;
using System;

namespace HttpFaker.Abstaction
{
    public static class NewtonsoftExtensions
    {
        public static FakeHttpClientOptions AddNewtonsoftJson(this FakeHttpClientOptions opt)
        {
            opt.JsonProvider = new NewtonsoftJsonProvider();

            return opt;
        }

        public static FakeHttpClientOptions AddNewtonsoftJson(this FakeHttpClientOptions opt, Action<JsonSerializerSettings> conf)
        {
            var jsonOpt = new JsonSerializerSettings();

            conf(jsonOpt);

            opt.JsonProvider = new NewtonsoftJsonProvider(jsonOpt);

            return opt;
        }
    }
}
