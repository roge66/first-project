using Microsoft.EntityFrameworkCore;
using UrlShortener.Services;

namespace UrlShortener.Persistence;

public sealed class AppDbContext : DbContext
{
    public DbSet<ShortLink> ShortLinks { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) :
        base(options)
    {
    }


    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ShortLinkConfiguration());
    }
    
}