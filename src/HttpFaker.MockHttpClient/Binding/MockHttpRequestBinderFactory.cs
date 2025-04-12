namespace HttpFaker.MockHttpClient.Binding
{
    internal class MockHttpRequestBinderFactory
    {
        public IMockHttpRequestBinder CreateBinder(string contentType)
        {
            switch (contentType)
            {
                case MimeTypes.ApplicationXml:
                case MimeTypes.ApplicationJson:
                    return new RawBinder();
                case MimeTypes.ApplicationFormUrlEncoded:
                    return new FormBinder();
                case MimeTypes.MultiPartFormData:
                    return new MultipartFormDataBinder();
                default:
                    return new NullBinder();
            }
        }
    }
}
