using Common;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Users.Core.Common;
using Users.Core.DTOs;
using Users.Core.Entities;
using Users.Core.Repositories;
using Users.Core.Services;
using Users.Infrastructure.Data;

namespace Users.Infrastructure.Services;

public class AuthService(
    IUserRepository userRepository,
    ILinkedAccountRepository linkedAccountRepository,
    IHashingService hashingService,
    ITokenService tokenService,
    UsersDbContext context,
    IStringLocalizer<AuthService> localizer,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<Result<TokenResponseDto>> LoginWithPasswordAsync(string email, string password,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetByEmailAsync(email, ct);
        if (user?.PasswordHash is null)
        {
            return Result<TokenResponseDto>.Fail(localizer["Error.Credentials.Invalid"], 401);
        }

        var isPasswordValid = hashingService.VerifyPassword(password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<TokenResponseDto>.Fail(localizer["Error.Credentials.Invalid"], 401);
        }

        var response = await GenerateTokensAsync(user, ct);
        return Result<TokenResponseDto>.Ok(response);
    }

    public async Task<Result<TokenResponseDto>> LoginWithOauthAsync(OauthProvider provider, string providerEmail,
        string providerUserId,
        CancellationToken ct = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        try
        {
            var user = await userRepository.GetByEmailAsync(providerEmail, ct);
            if (user is null)
            {
                user = new User
                {
                    Email = providerEmail,
                    Roles = ["User"],
                    EmailConfirmed = true
                };
                await userRepository.AddAsync(user, ct);
            }

            var existingLinkedAccount =
                await linkedAccountRepository.GetByProviderAndProviderUserIdAsync(provider, providerUserId, ct);
            if (existingLinkedAccount is null)
            {
                var linkedAccount = new LinkedAccount
                {
                    Provider = provider,
                    ProviderUserId = providerUserId,
                    UserId = user.Id
                };
                await linkedAccountRepository.AddAsync(linkedAccount, ct);
            }

            var response = await GenerateTokensAsync(user, ct);
            await transaction.CommitAsync(ct);
            return Result<TokenResponseDto>.Ok(response);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during OAuth login for provider {Provider} and user ID {ProviderUserId}",
                provider, providerUserId);
            await transaction.RollbackAsync(ct);
            return Result<TokenResponseDto>.Fail(localizer["Error.Internal"], 500);
        }
    }

    public async Task<Result<TokenResponseDto>> RefreshAuthAsync(string refreshToken, CancellationToken ct = default)
    {
        var isTokenValidResult = await tokenService.ValidateRefreshTokenAsync(refreshToken, ct);
        if (isTokenValidResult.IsFailure)
        {
            return Result<TokenResponseDto>.Fail(isTokenValidResult.Error, isTokenValidResult.StatusCode);
        }

        var user = await userRepository.GetByIdAsync(isTokenValidResult.Value, ct);
        if (user is null)
        {
            return Result<TokenResponseDto>.Fail(localizer["Error.RefreshToken.Invalid"], 401);
        }

        var response = await GenerateTokensAsync(user, ct);
        await tokenService.RevokeRefreshTokenAsync(refreshToken, ct);
        return Result<TokenResponseDto>.Ok(response);
    }

    private async Task<TokenResponseDto> GenerateTokensAsync(User user, CancellationToken ct = default)
    {
        var accessToken = tokenService.GenerateAccessToken(user.Id, user.Roles);
        var refreshToken = tokenService.GenerateRefreshToken();
        await tokenService.PersistRefreshTokenAsync(
            user.Id,
            refreshToken,
            DateTime.UtcNow.AddDays(7),
            ct);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            ExpiresIn = (int)TimeSpan.FromMinutes(15).TotalSeconds,
            RefreshToken = refreshToken,
            RefreshExpiresIn = (int)TimeSpan.FromDays(7).TotalSeconds
        };
    }
}