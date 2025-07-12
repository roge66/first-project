namespace UrlShortener.Services;

public sealed class ShortLinkCollisionException : Exception
{
    public ShortLinkCollisionException(string message) : base(message)
    {
    }
}