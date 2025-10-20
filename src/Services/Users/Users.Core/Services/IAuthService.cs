using Common;
using Users.Core.Common;
using Users.Core.DTOs;

namespace Users.Core.Services;

public interface IAuthService
{
    Task<Result<TokenResponseDto>>
        LoginWithPasswordAsync(string email, string password, CancellationToken ct = default);

    Task<Result<TokenResponseDto>> LoginWithOauthAsync(OauthProvider provider, string providerEmail,
        string providerUserId, CancellationToken ct = default);

    Task<Result<TokenResponseDto>> RefreshAuthAsync(string refreshToken, CancellationToken ct = default);
}