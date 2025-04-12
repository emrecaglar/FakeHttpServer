using System.Net.Http;
using System.Threading.Tasks;

namespace HttpFaker.MockHttpClient.Binding
{
    internal class NullBinder : IMockHttpRequestBinder
    {
        public Task BindAsync(HttpRequestMessage httpRequestMessage, MockHttpRequest mockHttpRequest)
        {
            return Task.CompletedTask;
        }
    }
}
