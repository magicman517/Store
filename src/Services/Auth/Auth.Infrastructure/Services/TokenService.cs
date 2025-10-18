using System.Security.Cryptography;
using System.Text;
using Auth.Application.Dtos.User;
using Auth.Application.Interfaces;
using Auth.Infrastructure.Data;
using Common;
using FastEndpoints.Security;

namespace Auth.Infrastructure.Services;

public class TokenService(AuthContext context) : ITokenService
{
    public string GenerateAccessToken(UserDto userDto)
    {
        return JwtBearer.CreateToken(o =>
        {
            o.ExpireAt = DateTime.UtcNow.AddMinutes(30);
            o.User.Roles.AddRange(userDto.Roles);
            o.User.Claims.Add(("sub", userDto.Id.ToString()));
            o.User.Claims.Add(("email", userDto.Email));
        });
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task PersistRefreshTokenAsync(Guid userId, string refreshToken, DateTimeOffset expiresAt,
        CancellationToken ct = default)
    {
        var hashed = HashToken(refreshToken);

        var tokenEntity = new RefreshToken
        {
            UserId = userId,
            Token = hashed,
            ExpiresAt = expiresAt,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.RefreshTokens.AddAsync(tokenEntity, ct);
        await context.SaveChangesAsync(ct);
    }

    public Task<Result<Guid>> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    private static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}