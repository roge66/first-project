using System.Collections.Concurrent;

namespace UrlShortener;

public sealed class ShortenedLinkStorage : IShortenedLinkStorage
{
    private readonly ConcurrentDictionary<string, string> _links = new();

    public Task AddAsync(string shortLink, string originalLink)
    {
        _links.TryAdd(shortLink, originalLink);
        return Task.CompletedTask;
    }

    public Task<string?> GetOriginalLinkAsync(string shortLink)
    {
        _links.TryGetValue(shortLink, out var originalLink);
        return Task.FromResult(originalLink);
    }
}