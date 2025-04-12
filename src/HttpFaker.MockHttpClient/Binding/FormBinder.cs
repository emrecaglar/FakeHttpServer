using HttpFaker.MockHttpClient.Internal;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpFaker.MockHttpClient.Binding
{
    internal class FormBinder : IMockHttpRequestBinder
    {
        public async Task BindAsync(HttpRequestMessage httpRequestMessage, MockHttpRequest mockHttpRequest)
        {
            string payload = null;

            if (httpRequestMessage.Content != null)
            {
                payload = await httpRequestMessage.Content.ReadAsStringAsync();
            }

            mockHttpRequest.Payload = payload;
            mockHttpRequest.Form = RequestPayload.ParseOrEmpty(payload);
        }
    }
}
