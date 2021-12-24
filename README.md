# Fake Http Server
It is used to respond to HttpClient requests for unit testing.


```csharp
var server = new FakeHttpServer();

var client = server.Setup(
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

    map => map.GET("/api/books", (req, res) =>
    {
        return res.Ok(new []
        {
            new{ name = "Helter Skelter", author = "Vincent Bugliosi", year = 1974 },
            new{ name = "Justice", author = "Dominic Dunne", year = 2001 },
            new{ name = "Devil's Knot", author = "Mara Leveritt", year = 2002 },
        });
    }),

    map => map.GET("/api/admin-area", (req, res) =>
    {
        return res.Json(new 
        { 
            name = "emre" 
        });
    }),

    map => map.GET("/api/users", (req, res) =>
    {
        res.AddHeader("Set-Cookie", "SessionId=123");

        return res.Json(new 
        { 
            name = "emre" 
        });
    }),

    map => map.GET("/api/users/.+", (req, res) =>
    {
        return res.Json(new 
        { 
            route = req.Route[2]
        });
    }),

    map => map.POST("/api/users", (req, res) =>
    {
        var createResult = new { UserId = 1 };

        return res.StatusCode((int)HttpStatusCode.Created, createResult);
    }),

    map => map.OPTIONS("/api/users"),

    map => map.GET("/api/downloadFile", (req, res) =>
    {
        var file = new byte[]{ 16, 24, 35 };

        return res.File(file);
    }),
+
    map => map.POST("/api/noroute", (req, res) =>
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