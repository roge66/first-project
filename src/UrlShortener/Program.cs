using UrlShortener;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("/api/shorten", (ShortenUrlRequest request) =>
{
    return Results.Ok(new ShortenUrlResponse(""));
});

app.MapGet("{shortCode}", (string shortCode) =>
{
    return Results.Ok($"{shortCode}");
});

app.Run();
