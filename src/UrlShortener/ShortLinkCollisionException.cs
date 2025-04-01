namespace UrlShortener;

public class ShortLinkCollisionException : Exception
{
    public ShortLinkCollisionException(string message) : base(message)
    {
    }
}