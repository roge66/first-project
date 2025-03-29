namespace UrlShortener;

public interface IShortenedLinkService
{
    Task<string> GenerateShortLinkAsync(string originalLink);
    Task<string?> GetOriginalLinkAsync(string shortLink);
}