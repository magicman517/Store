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
    /// <summary>
    /// Creates a JWT access token for the specified user, embedding the user's roles and claims and set to expire in 30 minutes.
    /// </summary>
    /// <param name="userDto">User information whose Id, Email, and Roles will be included in the token.</param>
    /// <returns>A JWT access token string that expires 30 minutes after issuance and includes the user's roles plus `sub` (user id) and `email` claims.</returns>
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

    /// <summary>
    /// Creates a cryptographically secure 32-byte random refresh token and encodes it as a Base64 string.
    /// </summary>
    /// <returns>A Base64-encoded string representing a 32-byte cryptographically secure random token.</returns>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Persists a hashed refresh token for the specified user with the provided expiration time.
    /// </summary>
    /// <param name="userId">The identifier of the user the refresh token belongs to.</param>
    /// <param name="refreshToken">The raw refresh token to be hashed and stored.</param>
    /// <param name="expiresAt">The expiration time for the refresh token.</param>
    /// <param name="ct">A cancellation token to cancel the persistence operation.</param>
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

    /// <summary>
    /// Validates the provided refresh token and returns the identifier of the associated user when valid.
    /// </summary>
    /// <param name="refreshToken">The refresh token to validate.</param>
    /// <returns>A <see cref="Result{Guid}"/> containing the user ID if the token is valid, or a failure Result describing why validation failed (for example, invalid or expired token).</returns>
    /// <exception cref="NotImplementedException">Thrown because the method is not yet implemented.</exception>
    public Task<Result<Guid>> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Revokes a previously issued refresh token so it can no longer be used for obtaining access tokens.
    /// </summary>
    /// <param name="refreshToken">The plaintext refresh token to revoke.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    public Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the SHA-256 hash of the provided token and returns the hash encoded as a Base64 string.
    /// </summary>
    /// <param name="token">The token string to hash.</param>
    /// <returns>The SHA-256 hash of <paramref name="token"/>, encoded as a Base64 string.</returns>
    private static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}