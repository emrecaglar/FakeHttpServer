using HttpFaker.Abstaction;
using System.Text.Json;

namespace HttpFaker.Serilization.SystemTextJson
{
    public class SystemTextJsonProvider : IJsonSeralizationProvider
    {
        private JsonSerializerOptions _jsonOptions;

        public SystemTextJsonProvider()
        {

        }

        public SystemTextJsonProvider(JsonSerializerOptions jsonOptions) : this()
        {
            _jsonOptions = jsonOptions;
        }

        public T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        public string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj, _jsonOptions);
        }
    }
}
