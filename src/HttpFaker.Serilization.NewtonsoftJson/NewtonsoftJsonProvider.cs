using HttpFaker.Abstaction;
using Newtonsoft.Json;

namespace HttpFaker.Serilization.NewtonsoftJson
{
    public class NewtonsoftJsonProvider : IJsonSeralizationProvider
    {
        private JsonSerializerSettings _jsonOptions;

        public NewtonsoftJsonProvider()
        {

        }

        public NewtonsoftJsonProvider(JsonSerializerSettings jsonOptions) : this()
        {
            _jsonOptions = jsonOptions;
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonOptions);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _jsonOptions);
        }
    }
}
