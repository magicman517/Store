using Auth.Application.Dtos.Auth.Responses;
using Auth.Application.Interfaces;
using Common;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.Services;

public class AuthService(
    IUserService userService,
    ITokenService tokenService,
    IStringLocalizer<AuthService> localizer,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<Result<TokenResponse>> AuthorizeAsync(string email, string password, CancellationToken ct = default)
    {
        logger.LogInformation("Login attempt for user: {Email}", email);
        
        var userResult = await userService.GetUserByEmailAsync(email, ct);
        if (userResult.IsFailure)
        {
            logger.LogWarning("Failed login attempt for user: {Email} - User not found", email);
            return Result<TokenResponse>.Fail(localizer["Error.Auth.InvalidCredentials"], 400);
        }

        var userDto = userResult.Value;

        var isPasswordValid = await userService.IsPasswordValidAsync(userDto, password, ct);
        if (!isPasswordValid)
        {
            logger.LogWarning("Failed login attempt for user: {Email} - Invalid password", email);
            return Result<TokenResponse>.Fail(localizer["Error.Auth.InvalidCredentials"], 400);
        }

        var accessToken = tokenService.GenerateAccessToken(userDto);
        var refreshToken = tokenService.GenerateRefreshToken();

        await tokenService.PersistRefreshTokenAsync(userDto.Id, refreshToken, DateTimeOffset.UtcNow.AddDays(7), ct);

        logger.LogInformation("Successful login for user: {Email}", email);
        
        var tokenResponse = new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        return Result<TokenResponse>.Ok(tokenResponse);
    }

    public async Task<Result<TokenResponse>> RefreshAuthAsync(string refreshToken, CancellationToken ct = default)
    {
        logger.LogInformation("Token refresh attempt");
        
        var refreshTokenDtoResult = await tokenService.GetRefreshTokenAsync(refreshToken, ct);
        if (refreshTokenDtoResult.IsFailure)
        {
            logger.LogWarning("Token refresh failed - Invalid refresh token");
            return Result<TokenResponse>.Fail(localizer["Error.Auth.InvalidRefreshToken"], 400);
        }

        var validateResult = await tokenService.ValidateRefreshTokenAsync(refreshToken, ct);
        if (validateResult.IsFailure)
        {
            logger.LogWarning("Token refresh failed - Refresh token validation failed for user: {UserId}", refreshTokenDtoResult.Value.UserId);
            return Result<TokenResponse>.Fail(localizer["Error.Auth.InvalidRefreshToken"], 400);
        }

        var userResult = await userService.GetUserByIdAsync(refreshTokenDtoResult.Value.UserId, ct);
        if (userResult.IsFailure)
        {
            logger.LogWarning("Token refresh failed - User not found: {UserId}", refreshTokenDtoResult.Value.UserId);
            return Result<TokenResponse>.Fail(localizer["Error.Auth.InvalidRefreshToken"], 404);
        }

        var userDto = userResult.Value;

        var newAccessToken = tokenService.GenerateAccessToken(userDto);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        // Revoke the old refresh token before persisting the new one
        await tokenService.RevokeRefreshTokenAsync(refreshToken, ct);
        await tokenService.PersistRefreshTokenAsync(userDto.Id, newRefreshToken, DateTimeOffset.UtcNow.AddDays(7), ct);
        
        logger.LogInformation("Token refresh successful for user: {UserId}", userDto.Id);
        
        var tokenResponse = new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
        return Result<TokenResponse>.Ok(tokenResponse);
    }
}