using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace HttpFaker.MockHttpClient.Tests.ExtensionsTests
{
    public class MockHttpRequestExtensionsTests
    {
        [Fact]
        public void TryGetQueryStringValues_ShouldBe_ReturnTrueHasKey()
        {
            var req = new MockHttpRequest
            {
                QueryString = new Dictionary<string, StringValues>
                {
                    { "q1", new StringValues(["val1", "val2"]) }
                }
            };

            var hasValue = req.TryGetQueryStringValues("q1", out var values);

            Assert.True(hasValue);
            Assert.Contains("val1", values.ToArray());
            Assert.Contains("val2", values.ToArray());
        }

        [Fact]
        public void TryGetQueryStringValues_ShouldBe_ReturnFalseHasNoKey_WhenQueryStringIsNull()
        {
            var req = new MockHttpRequest
            {
                QueryString = null
            };

            var hasValue = req.TryGetQueryStringValues("no-key", out _);

            Assert.False(hasValue);
        }

        [Fact]
        public void TryGetQueryStringValues_ShouldBe_ReturnFalseHasNoKey_WhenQueryStringIsEmpty()
        {
            var req = new MockHttpRequest();

            var hasValue = req.TryGetQueryStringValues("no-key", out _);

            Assert.False(hasValue);
        }

        [Fact]
        public void TryGetQueryStringValues_ShouldBe_ReturnFalseHasNoKey_WhenQueryStringHasNoKey()
        {
            var req = new MockHttpRequest 
            {
                QueryString = new Dictionary<string, StringValues>
                {
                    { "foo", new StringValues("bar") }
                }
            };

            var hasValue = req.TryGetQueryStringValues("no-key", out _);

            Assert.False(hasValue);
        }
    }
}
