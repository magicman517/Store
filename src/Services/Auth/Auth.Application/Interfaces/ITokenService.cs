using Auth.Application.Dtos.User;
using Common;

namespace Auth.Application.Interfaces;

public interface ITokenService
{
    /// <summary>
/// Creates an access token containing claims derived from the provided user data.
/// </summary>
/// <param name="userDto">User information used to populate the token's claims (identity and related metadata).</param>
/// <returns>A string representing the generated access token.</returns>
string GenerateAccessToken(UserDto userDto);
    /// <summary>
/// Generates a new cryptographically secure refresh token for client authentication.
/// </summary>
/// <returns>The generated refresh token string.</returns>
string GenerateRefreshToken();

    /// <summary>
/// Stores a refresh token for the specified user and records its expiration time.
/// </summary>
/// <param name="userId">The identifier of the user the refresh token belongs to.</param>
/// <param name="refreshToken">The refresh token string to persist.</param>
/// <param name="expiresAt">The date and time when the refresh token expires.</param>
/// <param name="ct">An optional cancellation token to cancel the operation.</param>
Task PersistRefreshTokenAsync(Guid userId, string refreshToken, DateTimeOffset expiresAt, CancellationToken ct = default);
    /// <summary>
/// Validates a refresh token and resolves the associated user identifier if valid.
/// </summary>
/// <param name="refreshToken">The refresh token to validate.</param>
/// <param name="ct">Optional cancellation token.</param>
/// <returns>A <see cref="Result{Guid}"/> containing the user's <see cref="Guid"/> when the token is valid, or a failed result otherwise.</returns>
Task<Result<Guid>> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>
/// Revokes the specified refresh token so it can no longer be used to obtain new access tokens.
/// </summary>
/// <param name="refreshToken">The refresh token to revoke.</param>
Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}