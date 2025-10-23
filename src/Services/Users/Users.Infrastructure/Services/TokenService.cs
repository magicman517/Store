using System.Security.Cryptography;
using System.Text;
using Common;
using FastEndpoints.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Users.Core.Entities;
using Users.Core.Repositories;
using Users.Core.Services;

namespace Users.Infrastructure.Services;

public class TokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IConfiguration configuration) : ITokenService
{
    private readonly string _jwtSigningKey = configuration["Jwt:SigningKey"] ??
                                             throw new InvalidOperationException("Jwt:SigningKey is not configured");

    public string GenerateAccessToken(Guid userId, IEnumerable<string> roles)
    {
        return JwtBearer.CreateToken(o =>
        {
            o.SigningKey = _jwtSigningKey;
            o.ExpireAt = DateTime.UtcNow.AddMinutes(15);

            o.User.Claims.Add(("sub", userId.ToString()));
            o.User.Claims.Add(("jti", Guid.NewGuid().ToString()));
            o.User.Roles.AddRange(roles);
        });
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    public async Task PersistRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiresAt,
        CancellationToken ct = default)
    {
        var hashedToken = HashToken(refreshToken);

        var token = new RefreshToken
        {
            Token = hashedToken,
            UserId = userId,
            ExpiresAt = expiresAt
        };

        await refreshTokenRepository.AddAsync(token, ct);
    }

    public async Task<Result<Guid>> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var hashedToken = HashToken(refreshToken);
        var token = await refreshTokenRepository.GetByTokenAsync(hashedToken, ct);

        if (token is null || token.ExpiresAt < DateTime.UtcNow)
        {
            return Result<Guid>.Fail("Недійсний токен оновлення", 401);
        }

        return Result<Guid>.Ok(token.UserId);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var hashedToken = HashToken(refreshToken);
        var token = await refreshTokenRepository.GetByTokenAsync(hashedToken, ct);

        if (token is not null)
        {
            await refreshTokenRepository.DeleteAsync(token, ct);
        }
    }

    private static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}