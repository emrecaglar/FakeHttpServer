using System.Net.Http;
using System.Threading.Tasks;

namespace HttpFaker.MockHttpClient.Binding
{
    internal interface IMockHttpRequestBinder
    {
        Task BindAsync(HttpRequestMessage httpRequestMessage, MockHttpRequest mockHttpRequest);
    }
}
