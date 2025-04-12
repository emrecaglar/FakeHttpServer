using HttpFaker.MockHttpClient.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpFaker.MockHttpClient.Binding
{
    internal class MultipartFormDataBinder : IMockHttpRequestBinder
    {
        public async Task BindAsync(HttpRequestMessage httpRequestMessage, MockHttpRequest mockHttpRequest)
        {
            string payload = null;

            if (httpRequestMessage.Content != null)
            {
                payload = await httpRequestMessage.Content.ReadAsStringAsync();
            }

            mockHttpRequest.Payload = payload;

            var files = new List<byte[]>();

            if (httpRequestMessage.Content is MultipartContent multipart)
            {
                var file = await multipart.ReadAsByteArrayAsync();

                foreach (var item in multipart)
                {
                    switch (item)
                    {
                        case FormUrlEncodedContent formUrlEncoded:
                            mockHttpRequest.Form = RequestPayload.ParseOrEmpty(await formUrlEncoded.ReadAsStringAsync());
                            break;
                        case StreamContent streamContent:
                            var bytes = await streamContent.ReadAsByteArrayAsync();
                            files.Add(bytes);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (files.Any())
            {
                mockHttpRequest.FormFiles = files;
            }
        }
    }
}
