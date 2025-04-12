using HttpFaker.Serilization.SystemTextJson;
using System;
using System.Text.Json;

namespace HttpFaker.Abstaction
{
    public static class SystemTextJsonExtensions
    {
        public static FakeHttpClientOptions AddSystemTextJson(this FakeHttpClientOptions opt)
        {
            opt.JsonProvider = new SystemTextJsonProvider(null);

            return opt;
        }

        public static FakeHttpClientOptions AddSystemTextJson(this FakeHttpClientOptions opt, Action<JsonSerializerOptions> conf)
        {
            var jsonOpt = new JsonSerializerOptions();

            conf(jsonOpt);

            opt.JsonProvider = new SystemTextJsonProvider(jsonOpt);

            return opt;
        }
    }
}
