using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Category>()
            .HasIndex(c => c.Slug)
            .IsUnique();

        modelBuilder
            .Entity<Product>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        modelBuilder
            .Entity<Category>()
            .HasIndex(c => c.Name);

        modelBuilder
            .Entity<Product>()
            .HasIndex(p => p.Name);
    }
}