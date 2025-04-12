using System.Collections.Generic;
using System;

namespace HttpFaker.MockHttpClient.Binding
{
    internal class MockHttpRequestBinderFactory
    {
        private readonly Dictionary<string, Type> _binders;

        public MockHttpRequestBinderFactory(Dictionary<string, Type> binders)
        {
            _binders = binders;
        }

        public IMockHttpRequestBinder CreateBinder(string contentType)
        {
            if (contentType != null && _binders.TryGetValue(contentType, out var binderType))
            {
                var binder = Activator.CreateInstance(binderType) as IMockHttpRequestBinder;

                return binder ?? new NullBinder();
            }

            return new NullBinder();
        }
    }
}
