using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpFaker.MockHttpClient.Binding
{
    internal class RawBinder : IMockHttpRequestBinder
    {
        public async Task BindAsync(HttpRequestMessage httpRequestMessage, MockHttpRequest mockHttpRequest)
        {
            string payload = null;

            if (httpRequestMessage.Content != null)
            {
                payload = await httpRequestMessage.Content.ReadAsStringAsync();
            }

            mockHttpRequest.Body = Encoding.UTF8.GetBytes(payload);
        }
    }
}
