using Common;

namespace Users.Core.Services;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, IEnumerable<string> roles);
    string GenerateRefreshToken();

    Task PersistRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiresAt, CancellationToken ct = default);
    Task<Result<Guid>> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}