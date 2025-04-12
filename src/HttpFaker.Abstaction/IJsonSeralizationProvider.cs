using System;

namespace HttpFaker.Abstaction
{
    public interface IJsonSeralizationProvider
    {
        string Serialize(object obj);

        T Deserialize<T>(string json);
    }
}
