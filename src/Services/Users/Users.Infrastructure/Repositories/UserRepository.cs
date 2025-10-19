using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Users.Core.Entities;
using Users.Core.Repositories;
using Users.Infrastructure.Data;

namespace Users.Infrastructure.Repositories;

public class UserRepository(ILogger<UserRepository> logger, UsersContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("User {Email} has been added", user.Email);
    }

    public async Task AddToRoleAsync(User user, string role, CancellationToken ct = default)
    {
        if (user.Roles.Contains(role))
        {
            logger.LogWarning("User {Email} is already in role {Role}", user.Email, role);
            return;
        }
        user.Roles.Add(role);
        context.Users.Update(user);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("User {Email} has been added to role {Role}", user.Email, role);
    }

    public async Task RemoveFromRoleAsync(User user, string role, CancellationToken ct = default)
    {
        user.Roles.Remove(role);
        context.Users.Update(user);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("User {Email} has been removed from role {Role}", user.Email, role);
    }
}