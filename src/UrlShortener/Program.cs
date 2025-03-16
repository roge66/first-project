using Microsoft.OpenApi.Models;
using UrlShortener;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UrlShortener API",
        Version = "v1",
        Description = "API для сокращения URL-адресов"
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "UrlShortener API");
    });
}

app.MapPost("/shorten", (ShortenUrlRequest request) =>
{
    return Results.Ok(new ShortenUrlResponse(""));
});

app.MapGet("{shortCode}", (string shortCode) =>
{
    return Results.Ok($"{shortCode}");
});

app.Run();
