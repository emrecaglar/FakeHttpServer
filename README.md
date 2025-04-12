# Fake Http Server
It is used to respond to HttpClient requests for unit testing.

![NuGet Version](https://img.shields.io/nuget/v/FakeHttpServer)
![NuGet Downloads](https://img.shields.io/nuget/dt/FakeHttpServer)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/emrecaglar/FakeHttpServer/dotnet.yml)


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

    map => map.MapGet("/api/admin-area", (req, res) =>
    {
        return res.Json(new 
        { 
            name = "emre" 
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

    map => map.MapGet("/api/users/.+", (req, res) =>
    {
        return res.Json(new 
        { 
            route = req.Route[2]
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
    }),
+
    map => map.MapPost("/api/noroute", (req, res) =>
    {
        return res.StatusCode(404);
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
