using Auth.Application.Dtos.Auth.Responses;
using Auth.Application.Dtos.User;
using Auth.Application.Dtos.User.Responses;
using Common;

namespace Auth.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(UserDto userDto);
    string GenerateRefreshToken();

    Task<Result<RefreshTokenDto>> GetRefreshTokenAsync(string refreshToken, CancellationToken ct = default);

    Task PersistRefreshTokenAsync(Guid userId, string refreshToken, DateTimeOffset expiresAt, CancellationToken ct = default);
    Task<Result<bool>> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default);

    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}