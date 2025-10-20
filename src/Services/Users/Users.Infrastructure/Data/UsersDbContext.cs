using Microsoft.EntityFrameworkCore;
using Users.Core.Entities;

namespace Users.Infrastructure.Data;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<LinkedAccount> LinkedAccounts { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder
            .Entity<LinkedAccount>()
            .Property(la => la.Provider)
            .HasConversion<string>();
    }
}