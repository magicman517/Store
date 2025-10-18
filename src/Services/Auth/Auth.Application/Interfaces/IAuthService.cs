using Auth.Application.Dtos.Auth.Responses;
using Common;

namespace Auth.Application.Interfaces;

public interface IAuthService
{
    Task<Result<TokenResponse>> AuthorizeAsync(string email, string password, CancellationToken ct = default);
    Task<Result<TokenResponse>> RefreshAuthAsync(string refreshToken, CancellationToken ct = default);
}