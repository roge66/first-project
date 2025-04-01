using Microsoft.OpenApi.Models;
using UrlShortener;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IShortenedLinkStorage, ShortenedLinkStorage>();
builder.Services.AddScoped<IShortenedLinkService, ShortenedLinkService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "UrlShortener API");
    });
}

app.MapPost("/shorten", async (
    ShortenUrlRequest request, 
    IShortenedLinkService service,
    CancellationToken cancellationToken) =>
{
    if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("Invalid url");
    }
    
    var shortLink = await service.GenerateShortLinkAsync(request.Url, cancellationToken);
    return Results.Ok(new ShortenUrlResponse($"{app.Urls.First()}/{shortLink}"));
});

app.MapGet("{shortCode}", async (
    string shortCode, 
    IShortenedLinkService service,
    CancellationToken cancellationToken) =>
{
    var originalUrl = await service.GetOriginalLinkAsync(shortCode, cancellationToken);
    return originalUrl is null 
        ? Results.NotFound() 
        : Results.Redirect(originalUrl);
});

app.Run();
