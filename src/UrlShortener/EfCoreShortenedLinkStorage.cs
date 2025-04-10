using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace UrlShortener;

public sealed class EfCoreShortenedLinkStorage : IShortenedLinkStorage
{
    private readonly AppDbContext _context;

    public EfCoreShortenedLinkStorage(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(string shortLink, string originalLink, CancellationToken cancellationToken)
    {
        var link = new ShortLink
        {
            Id = Guid.NewGuid(),
            ShortCode = shortLink,
            OriginalUrl = originalLink,
            // CreatedAt = DateTime.Now
        };
        
        try
        {
            await _context.ShortLinks.AddAsync(link, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new ShortLinkCollisionException($"Short link {shortLink} already exists.");
        }
    }

    public async Task<string?> GetOriginalLinkAsync(string shortLink, CancellationToken cancellationToken)
    {
        return await _context.ShortLinks
            .Where(s => s.ShortCode == shortLink)
            .Select(s => s.OriginalUrl)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException { SqlState: "23505" };
    }
}