using System.Security.Cryptography;
using System.Text;
using Auth.Application.Dtos.Auth.Responses;
using Auth.Application.Dtos.User.Responses;
using Auth.Application.Interfaces;
using Auth.Infrastructure.Data;
using Common;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Auth.Infrastructure.Services;

public class TokenService(IStringLocalizer<TokenService> localizer, AuthContext context) : ITokenService
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

    public async Task<Result<RefreshTokenDto>> GetRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var hashed = HashToken(refreshToken);

        var tokenEntity = await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == hashed, ct);

        if (tokenEntity == null)
        {
            return Result<RefreshTokenDto>.Fail(localizer["Error.RefreshToken.NotFound"], 404);
        }

        var refreshTokenDto = new RefreshTokenDto
        {
            Token = tokenEntity.Token,
            UserId = tokenEntity.UserId,
            ExpiresAt = tokenEntity.ExpiresAt
        };

        return Result<RefreshTokenDto>.Ok(refreshTokenDto);
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

    public async Task<Result<bool>> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var hashed = HashToken(refreshToken);

        var tokenEntity = await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == hashed, ct);

        if (tokenEntity == null || tokenEntity.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return Result<bool>.Fail(localizer["Error.RefreshToken.Invalid"], 400);
        }

        return Result<bool>.Ok(true);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var hashed = HashToken(refreshToken);

        var tokenEntity = await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == hashed, ct);

        if (tokenEntity != null)
        {
            context.RefreshTokens.Remove(tokenEntity);
            await context.SaveChangesAsync(ct);
        }
    }

    private static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}