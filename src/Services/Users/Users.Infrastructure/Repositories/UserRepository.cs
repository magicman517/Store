using Microsoft.EntityFrameworkCore;
using Users.Core.Entities;
using Users.Core.Repositories;
using Users.Infrastructure.Data;

namespace Users.Infrastructure.Repositories;

public class UserRepository(UsersDbContext context) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Users
            .AsNoTracking()
            .Include(u => u.LinkedAccounts)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await context.Users
            .AsNoTracking()
            .Include(u => u.LinkedAccounts)
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(ct);
    }

    public async Task<bool> AnyUserExistsAsync(CancellationToken ct = default)
    {
        return await context.Users.AnyAsync(ct);
    }
}