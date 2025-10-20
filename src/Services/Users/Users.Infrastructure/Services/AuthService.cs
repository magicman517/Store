using Common;
using Microsoft.Extensions.Localization;
using Users.Core.Common;
using Users.Core.DTOs;
using Users.Core.Entities;
using Users.Core.Repositories;
using Users.Core.Services;

namespace Users.Infrastructure.Services;

public class AuthService(
    IUserRepository userRepository,
    ILinkedAccountRepository linkedAccountRepository,
    IHashingService hashingService,
    ITokenService tokenService,
    IStringLocalizer<AuthService> localizer) : IAuthService
{
    public async Task<Result<TokenResponseDto>> LoginWithPasswordAsync(string email, string password,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetByEmailAsync(email, ct);
        if (user?.PasswordHash is null)
        {
            return Result<TokenResponseDto>.Fail(localizer["Error.InvalidCredentials"], 401);
        }

        var isPasswordValid = hashingService.VerifyPassword(password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<TokenResponseDto>.Fail(localizer["Error.InvalidCredentials"], 401);
        }

        var response = await GenerateTokensAsync(user, ct);
        return Result<TokenResponseDto>.Ok(response);
    }

    public async Task<Result<TokenResponseDto>> LoginWithOauthAsync(OauthProvider provider, string providerEmail,
        string providerUserId,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetByEmailAsync(providerEmail, ct);
        if (user is null)
        {
            user = new User
            {
                Email = providerEmail,
                Roles = ["User"]
            };
            await userRepository.AddAsync(user, ct);

            var linkedAccount = new LinkedAccount
            {
                Provider = provider,
                ProviderUserId = providerUserId,
                UserId = user.Id
            };
            await linkedAccountRepository.AddAsync(linkedAccount, ct);
        }

        var response = await GenerateTokensAsync(user, ct);
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