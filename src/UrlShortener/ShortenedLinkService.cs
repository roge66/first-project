namespace UrlShortener;

public sealed class ShortenedLinkService : IShortenedLinkService
{
    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int ShortLinkLength = 8;
    private readonly IShortenedLinkStorage _storage;
    private readonly Random _random = new();
    
    public ShortenedLinkService(IShortenedLinkStorage storage)
    {
        _storage = storage;
    }

    public async Task<string> GenerateShortLinkAsync(string originalLink, CancellationToken cancellationToken)
    {
        const int maxAttempts = 5;
        string shortCode;
        bool addedSuccessfully;
        int attempts = 0;
        
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            shortCode = GenerateShortCode();
            attempts++;
            try
            {
                await _storage.AddAsync(shortCode, originalLink, cancellationToken);
                addedSuccessfully = true;
            }
            catch (ShortLinkCollisionException)
            {
                addedSuccessfully = false;
                if (attempts >= maxAttempts)
                {
                    throw new InvalidOperationException("Failed to generate short-link after multiple attempts.");
                }
            }
        } while (!addedSuccessfully && (attempts < maxAttempts));
        //await _storage.AddAsync(shortCode, originalLink, cancellationToken);
        return shortCode;
    }

    public Task<string?> GetOriginalLinkAsync(string shortCode, CancellationToken cancellationToken)
    {
        return _storage.GetOriginalLinkAsync(shortCode, cancellationToken);
    }

    private string GenerateShortCode()
    {
        return new string(Enumerable
            .Repeat(Characters, ShortLinkLength)
            .Select(s => s[_random.Next(s.Length)])
            .ToArray());
    }
}