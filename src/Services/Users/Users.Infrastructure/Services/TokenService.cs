using Users.Core.Services;
using Users.Infrastructure.Data;

namespace Users.Infrastructure.Services;

public class TokenService(UsersDbContext context) : ITokenService
{
    public string GenerateAccessToken(Guid userId, IEnumerable<string> roles)
    {
        throw new NotImplementedException();
    }

    public string GenerateRefreshToken()
    {
        throw new NotImplementedException();
    }

    public Task PersistRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiresAt, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task RevokeRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}