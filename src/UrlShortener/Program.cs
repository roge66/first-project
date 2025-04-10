using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UrlShortener;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration["DbConnectionString"]!));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IShortenedLinkStorage, EfCoreShortenedLinkStorage>();
builder.Services.AddScoped<IShortenedLinkService, ShortenedLinkService>();

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

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
