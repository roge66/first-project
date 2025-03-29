namespace UrlShortener;

public interface IShortenedLinkStorage
{
    Task AddAsync(string shortLink, string originalLink);
    Task<string?> GetOriginalLinkAsync(string shortLink);
}