namespace UrlShortener;

public sealed class ShortenedLinkService : IShortenedLinkService
{
    private readonly IShortenedLinkStorage _storage;
    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int ShortLinkLength = 8;
    private readonly Random _random = new();
    
    public ShortenedLinkService(IShortenedLinkStorage storage)
    {
        _storage = storage;
    }

    public async Task<string> GenerateShortLinkAsync(string originalLink)
    {
        string shortCode;
        do
        {
            shortCode = GenerateShortCode();
        } while (await _storage.GetOriginalLinkAsync(shortCode) != null);
        await _storage.AddAsync(shortCode, originalLink);
        return shortCode;
    }

    public Task<string?> GetOriginalLinkAsync(string shortCode)
    {
        return _storage.GetOriginalLinkAsync(shortCode);
    }

    private string GenerateShortCode()
    {
        return new string(Enumerable.Repeat(Characters, ShortLinkLength)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}