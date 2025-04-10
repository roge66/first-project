using Microsoft.EntityFrameworkCore;

namespace UrlShortener;

public class AppDbContext : DbContext
{
    public DbSet<ShortLink> ShortLinks { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) :
        base(options)
    {
    }


    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortLink>()
            .HasIndex(x => x.ShortCode)
            .IsUnique();
    }
}