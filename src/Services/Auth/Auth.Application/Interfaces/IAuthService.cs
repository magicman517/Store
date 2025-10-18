using Auth.Application.Dtos.Auth.Responses;
using Common;

namespace Auth.Application.Interfaces;

public interface IAuthService
{
    /// <summary>
/// Authorizes a user by credentials and produces an authentication token response.
/// </summary>
/// <returns>A Result containing a TokenResponse on success, or a failure Result with error details.</returns>
Task<Result<TokenResponse>> AuthorizeAsync(string email, string password, CancellationToken ct = default);
}