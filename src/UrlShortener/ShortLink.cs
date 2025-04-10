namespace UrlShortener;

public class ShortLink
{
    public Guid Id { get; set; }
    public required string ShortCode { get; set; }
    public required string OriginalUrl { get; set; }
    // public DateTime CreatedAt {get; set; }
}

