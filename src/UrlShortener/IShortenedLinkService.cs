namespace UrlShortener;

public interface IShortenedLinkService
{
    Task<string> GenerateShortLinkAsync(string originalLink, CancellationToken cancellationToken);
    Task<string?> GetOriginalLinkAsync(string shortLink, CancellationToken cancellationToken);
}