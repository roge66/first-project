namespace UrlShortener;

public interface IShortenedLinkStorage
{
    Task AddAsync(string shortLink, string originalLink, CancellationToken cancellationToken);
    Task<string?> GetOriginalLinkAsync(string shortLink, CancellationToken cancellationToken);
}