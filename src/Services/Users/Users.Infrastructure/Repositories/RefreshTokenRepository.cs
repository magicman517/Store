using Microsoft.EntityFrameworkCore;
using Users.Core.Entities;
using Users.Core.Repositories;
using Users.Infrastructure.Data;

namespace Users.Infrastructure.Repositories;

public class RefreshTokenRepository(UsersDbContext context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        await context.RefreshTokens.AddAsync(refreshToken, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token, ct);
    }

    public async Task DeleteAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        context.RefreshTokens.Remove(refreshToken);
        await context.SaveChangesAsync(ct);
    }
}