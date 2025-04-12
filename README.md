# Fake Http Server
It is used to respond to HttpClient requests for unit testing.

![NuGet Version](https://img.shields.io/nuget/v/HttpFaker.MockHttpClient?style=for-the-badge)
![NuGet Downloads](https://img.shields.io/nuget/dt/HttpFaker.MockHttpClient?style=for-the-badge)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/emrecaglar/FakeHttpServer/dotnet.yml?style=for-the-badge)
![GitHub License](https://img.shields.io/github/license/emrecaglar/FakeHttpServer?style=for-the-badge)

## FakeHttpServer

### Overview
- **Purpose**: It simulates HTTP server responses for unit testing purposes, allowing developers to test their HTTP client code without needing a real server.
- **Language**: The project is written in C#.

### Features
- **Middleware Setup**: You can set up middleware to handle requests and responses, including authorization checks.
- **Route Mapping**: It supports various HTTP methods (GET, POST, PUT, OPTIONS, TRACE, HEAD, DELETE) and routes, allowing you to define how the server should respond to different endpoints.
- **Response Handling**: You can customize responses, including status codes, headers, and content.

### Example Usage
Here's a brief example of how you might use FakeHttpServer:

```csharp
var fakeHttp = new FakeHttpClient();

var client = fakeHttp.Setup(
    map => map.UseMiddleware((req, res, next) => 
    {
        bool exists = req.Headers.TryGetValue(HeaderNames.Authorization, out string header)

        if(exits && header.Equals("Bearer true-jwt", StringComparison.OrdinalIgnoreCase))
        {
            next();
        }
        else
        {
            res.StatusCode((int)HttpStatusCode.Unauthorized);
        }
    }),

	map => map.MapGet(@"/api/book/([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})/page/(\d+)", (req, res) =>
	{
		return res.Ok(new BookPage
		{
			BookId = Guid.Parse(req.Route[2]),
			PageNum = int.Parse(req.Route[4])
		});
	})

    map => map.MapGet("/api/books", (req, res) =>
    {
        return res.Ok(new []
        {
            new{ name = "Helter Skelter", author = "Vincent Bugliosi", year = 1974 },
            new{ name = "Justice", author = "Dominic Dunne", year = 2001 },
            new{ name = "Devil's Knot", author = "Mara Leveritt", year = 2002 },
        });
    }),

    map => map.MapGet("/api/users", (req, res) =>
    {
        res.AddHeader("Set-Cookie", "SessionId=123");

        return res.Json(new 
        { 
            name = "emre" 
        });
    }),

    map => map.MapPost("/api/users", (req, res) =>
    {
        var createResult = new { UserId = 1 };

        return res.StatusCode((int)HttpStatusCode.Created, createResult);
    }),

    map => map.MapOptions("/api/users"),

    map => map.MapGet("/api/downloadFile", (req, res) =>
    {
        var file = new byte[]{ 16, 24, 35 };

        return res.File(file);
    })
);

var result = await client.GetAsync("/api/books");

string content = await  result.Content.ReadAsStringAsync();
```

```json
//Response Content:
[
    { "name": "Helter Skelter", "author": "Vincent Bugliosi", "year": 1974 },
    { "name": "Justice", "author": "Dominic Dunne", "year": 2001 },
    { "name": "Devil's Knot", "author": "Mara Leveritt", "year": 2002 }
]
```

# Summary
<details open><summary>Summary</summary>

|Title|Value|
|:---|:---|
| Generated on: | 12.04.2025 - 20:38:24 |
| Coverage date: | 12.04.2025 - 20:29:22 |
| Parser: | Cobertura |
| Assemblies: | 2 |
| Classes: | 16 |
| Files: | 15 |
| **Line coverage:** | 73.1% (313 of 428) |
| Covered lines: | 313 |
| Uncovered lines: | 115 |
| Coverable lines: | 428 |
| Total lines: | 805 |
| **Branch coverage:** | 71.7% (56 of 78) |
| Covered branches: | 56 |
| Total branches: | 78 |
| **Method coverage:** | [Feature is only available for sponsors](https://reportgenerator.io/pro) |

</details>

## Coverage
<details><summary>HttpFaker.Abstaction - 100%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**HttpFaker.Abstaction**|**100%**|****|
|HttpFaker.Abstaction.FakeHttpClientOptions|100%||

</details>
<details><summary>HttpFaker.MockHttpClient - 72.9%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**HttpFaker.MockHttpClient**|**72.9%**|**71.7%**|
|HttpFaker.MockHttpClient.Binding.FormBinder|100%|100%|
|HttpFaker.MockHttpClient.Binding.MockHttpRequestBinderFactory|100%|83.3%|
|HttpFaker.MockHttpClient.Binding.MultipartFormDataBinder|96.2%|100%|
|HttpFaker.MockHttpClient.Binding.NullBinder|100%||
|HttpFaker.MockHttpClient.Binding.RawBinder|100%|100%|
|HttpFaker.MockHttpClient.Extensions.FakeHttpClientOptionsExtensions|0%|0%|
|HttpFaker.MockHttpClient.FakeHttpClient|89.4%||
|HttpFaker.MockHttpClient.Internal.PayloadInput|100%||
|HttpFaker.MockHttpClient.Internal.RegexRouteMatcher|100%|100%|
|HttpFaker.MockHttpClient.Internal.RequestPayload|88.8%|83.3%|
|HttpFaker.MockHttpClient.Internal.RouteCollection|75%||
|HttpFaker.MockHttpClient.MockHttpMessageHandler|100%|85.7%|
|HttpFaker.MockHttpClient.MockHttpRequest|100%||
|HttpFaker.MockHttpClient.MockHttpResponse|52.4%|61.5%|
|HttpFaker.MockHttpClient.RequestHandler|56.1%|0%|

</details>
