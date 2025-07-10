using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UrlShortener;

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

    public class ShortLinkConfiguration : IEntityTypeConfiguration<ShortLink>
    {
        public void Configure(EntityTypeBuilder<ShortLink> builder)
        {
            builder.HasIndex(x => x.ShortCode).IsUnique();
        }
    }
}