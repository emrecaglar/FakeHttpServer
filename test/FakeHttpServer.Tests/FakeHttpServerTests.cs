using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using System.Collections.Generic;
using System.Net.Http;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace FakeHttpServer.Tests
{
    public class FakeHttpServerTests
    {
        [Fact]
        public async Task Ok_Response_ShouldBe_Return_SuccessStatusCode_And_Content()
        {
            var server = new FakeHttpClient();

            var client = server.Setup(
                map => map.GET("/api/books", (req, res) =>
                {
                    return res.Ok(new[]
                    {
                        new{ name = "Helter Skelter", author = "Vincent Bugliosi", year = 1974 },
                        new{ name = "Justice", author = "Dominic Dunne", year = 2001 },
                        new{ name = "Devil's Knot", author = "Mara Leveritt", year = 2002 },
                    });
                })
            );

            var result = await client.GetAsync("/api/books");

            string content = await result.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<List<Book>>(content);

            Assert.Equal(3, responseObject.Count);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Route_Regex_ShouldBe_Return_RouteValue()
        {
            var server = new FakeHttpClient();

            var client = server.Setup(
                map => map.GET("/api/users/.+", (req, res) =>
                {
                    return res.Ok(new { route = req.Route[2] });
                })
            );

            string routeValue = Guid.NewGuid().ToString();

            var result = await client.GetAsync($"/api/users/{routeValue}");

            string content = await result.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<dynamic>(content);

            Assert.Equal(routeValue, responseObject.route.ToString());
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Login_ShouldBe_Return_SetCookie_In_Header()
        {
            var server = new FakeHttpClient();

            var client = server.Setup(
                map => map.POST("/api/users/signin", (req, res) =>
                {
                    res.AddHeader("Set-Cookie", "SessionId=123");
                    int i = req.Body.id;
                    return res.Ok();
                })
            );

            var result = await client.PostAsync($"/api/users/signin", new StringContent("{ \"id\": 1 }", Encoding.UTF8, "application/json"));

            bool ok = result.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> headers);

            var setCookieHeader = headers.FirstOrDefault();

            Assert.NotNull(setCookieHeader);
            Assert.Equal("SessionId=123", setCookieHeader);
            Assert.True(ok);
        }

        [Fact]
        public async Task Options_Request_ShouldBe_Return_Usable_HttpMethods()
        {
            var server = new FakeHttpClient();

            var client = server.Setup(
                map => map.GET("/api/users", (req, res) =>
                {
                    return res.Ok();
                }),

                map => map.POST("/api/users", (req, res) =>
                {
                    return res.Ok();
                }),

                map => map.OPTIONS("/api/users")
            );

            var result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/api/users"));

            bool ok = result.Headers.TryGetValues("Access-Control-Allow-Methods", out IEnumerable<string> headerValues);

            var header = headerValues.FirstOrDefault();

            var usablehttpMethods = header.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            Assert.True(ok);

            Assert.NotNull(header);

            Assert.Contains("GET", usablehttpMethods, new StringArrayIgnoreCaseEqualityComparer());
            Assert.Contains("POST", usablehttpMethods, new StringArrayIgnoreCaseEqualityComparer());
        }

        [Fact]
        public async Task GetStream_Request_ShouldBe_Return_File()
        {
            var server = new FakeHttpClient();

            var file = new byte[] { 16, 33, 22 };

            var client = server.Setup(
                map => map.GET("/api/file", (req, res) =>
                {
                    return res.File(file);
                })
            );

            var stream = await client.GetStreamAsync("api/file");

            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            var downloadedFile = memoryStream.ToArray();

            var isEqual = file.SequenceEqual(downloadedFile);

            Assert.True(isEqual);
        }

        [Fact]
        public async Task Request_ShouldBe_Return_BadRequest()
        {
            var server = new FakeHttpClient();

            var client = server.Setup(
                map => map.GET("/api/some", (req, res) =>
                {
                    return res.StatusCode((int)HttpStatusCode.BadRequest);
                })
            );

            var response = await client.GetAsync("api/some");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Request_ShouldBe_Return_404()
        {
            var server = new FakeHttpClient();

            var client = server.Setup(
                map => map.GET("/api/some", (req, res) =>
                {
                    return res.Ok();
                })
            );

            var response = await client.GetAsync("api/does-not-exist");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("api/authenticated-required", "true-jwt", "admin", HttpStatusCode.OK)]
        [InlineData("api/admin", "true-jwt", "admin", HttpStatusCode.OK)]
        [InlineData("api/admin", "true-jwt", "user", HttpStatusCode.Forbidden)]
        [InlineData("api/authenticated-required", "wrong-jwt", "", HttpStatusCode.Unauthorized)]
        public async Task UseMiddleware_Request_ShouldBe_Check_Jwt_Return_True_StatusCode(string resource, string jwt, string role, HttpStatusCode expectedStatusCode)
        {
            var server = new FakeHttpClient();

            var client = server.Setup(
                //Authentication
                map => map.UseMiddleware((req, res, next) =>
                {
                    if (req.Headers.TryGetValue(HeaderNames.Authorization, out string authorizationHeader) && authorizationHeader.Equals("Bearer true-jwt", StringComparison.OrdinalIgnoreCase))
                    {
                        next();

                        return;
                    }

                    res.StatusCode((int)HttpStatusCode.Unauthorized);
                }),

                //Authorization
                map => map.UseMiddleware((req, res, next) =>
                {
                    if (req.Uri.AbsoluteUri.Contains("/admin") && role == "user")
                    {
                        res.StatusCode((int)HttpStatusCode.Forbidden);
                    }
                    else
                    {
                        next();
                    }
                }),

                map => map.GET("/api/authenticated-required", (req, res) =>
                {
                    return res.Ok();
                }),

                map => map.GET("/api/admin", (req, res) =>
                {
                    return res.Ok();
                }),

                map => map.GET("/api/user", (req, res) =>
                {
                    return res.Ok();
                })
            );

            client.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.Authorization, $"Bearer {jwt}");

            var authenticationRequiredResponse = await client.GetAsync(resource);

            Assert.Equal(expectedStatusCode, authenticationRequiredResponse.StatusCode);
        }
    }

    public class StringArrayIgnoreCaseEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals([AllowNull] string x, [AllowNull] string y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return obj.GetHashCode();
        }
    }

    public class Book
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public string Year { get; set; }
    }
}