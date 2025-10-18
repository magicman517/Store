using Auth.Application.Dtos.Auth.Responses;
using Auth.Application.Interfaces;
using Common;
using Microsoft.Extensions.Localization;

namespace Auth.Infrastructure.Services;

public class AuthService(IUserService userService, ITokenService tokenService, IStringLocalizer<AuthService> localizer) : IAuthService
{
    /// <summary>
    /// Authenticates a user by email and password and issues an access token and a refresh token.
    /// </summary>
    /// <param name="email">The user's email address used for lookup.</param>
    /// <param name="password">The plaintext password to validate against the user's stored credentials.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>`Result&lt;TokenResponse&gt;` containing the issued `AccessToken` and `RefreshToken` on success; a failed result with a localized "Error.Auth.InvalidCredentials" message and HTTP status code 400 if authentication fails.</returns>
    public async Task<Result<TokenResponse>> AuthorizeAsync(string email, string password, CancellationToken ct = default)
    {
        var userResult = await userService.GetUserByEmailAsync(email, ct);
        if (userResult.IsFailure)
        {
            return Result<TokenResponse>.Fail(localizer["Error.Auth.InvalidCredentials"], 400);
        }

        var userDto = userResult.Value;

        var isPasswordValid = await userService.IsPasswordValidAsync(userDto, password, ct);
        if (!isPasswordValid)
        {
            return Result<TokenResponse>.Fail(localizer["Error.Auth.InvalidCredentials"], 400);
        }

        var accessToken = tokenService.GenerateAccessToken(userDto);
        var refreshToken = tokenService.GenerateRefreshToken();

        await tokenService.PersistRefreshTokenAsync(userDto.Id, refreshToken, DateTimeOffset.UtcNow.AddDays(7), ct);

        var tokenResponse = new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        return Result<TokenResponse>.Ok(tokenResponse);
    }
}