using Auth.Core.DTOs;
using Common;

namespace Auth.Core.Services;

public interface IAuthService
{
    AuthUrlResponseDto GetAuthUrl();
    Task<Result<TokensResponseDto>> ExchangeCodeAsync(string code, CancellationToken ct = default);
    Task<Result<TokensResponseDto>> RefreshAuthAsync(string refreshToken, CancellationToken ct = default);
}