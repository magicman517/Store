using Auth.Application.Dtos.User;
using Common;

namespace Auth.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(UserDto userDto);
    string GenerateRefreshToken();

    Task PersistRefreshTokenAsync(Guid userId, string refreshToken, DateTimeOffset expiresAt, CancellationToken ct = default);
    Task<Result<Guid>> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default);

    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}