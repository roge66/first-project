using System.Collections.Concurrent;

namespace UrlShortener.Services;

public sealed class ShortenedLinkStorage : IShortenedLinkStorage
{
    private readonly ConcurrentDictionary<string, string> _shortLinkToOriginalLinkMap = new();

    public Task AddAsync(string shortLink, string originalLink, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_shortLinkToOriginalLinkMap.TryAdd(shortLink, originalLink))
        {
            throw new ShortLinkCollisionException($"Short link {shortLink} already exists.");
        }
        return Task.CompletedTask;
    }

    public Task<string?> GetOriginalLinkAsync(string shortLink, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var originalLink = _shortLinkToOriginalLinkMap.GetValueOrDefault(shortLink, null);
        return Task.FromResult(originalLink);
    }
}