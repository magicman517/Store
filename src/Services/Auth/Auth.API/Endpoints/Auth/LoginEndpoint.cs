using Auth.Application.Dtos.Auth.Requests;
using Auth.Application.Dtos.Auth.Responses;
using Auth.Application.Interfaces;
using FastEndpoints;

namespace Auth.API.Endpoints.Auth;

public class LoginEndpoint(IAuthService authService) : Endpoint<LoginRequest, TokenResponse>
{
    /// <summary>
    /// Configures the endpoint's route and access policy.
    /// </summary>
    /// <remarks>
    /// Registers a POST route at "/auth/login" and allows anonymous access to the endpoint.
    /// </remarks>
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    /// <summary>
    /// Authenticates the provided login credentials and produces a token response for successful logins.
    /// </summary>
    /// <param name="req">The login request containing the user's email and password.</param>
    /// <remarks>
    /// On authentication failure, the endpoint terminates handling with an error response using the authorization result's error and status code. On success, the endpoint populates the response with access and refresh tokens and their expiration intervals, then returns a 200 OK with the token data.
    /// </remarks>
    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var authResult = await authService.AuthorizeAsync(req.Email, req.Password, ct);
        if (authResult.IsFailure)
        {
            ThrowError(authResult.Error, authResult.StatusCode);
        }

        Response = new TokenResponse
        {
            AccessToken = authResult.Value.AccessToken,
            AccessTokenExpiresIn = authResult.Value.AccessTokenExpiresIn,
            RefreshToken = authResult.Value.RefreshToken,
            RefreshTokenExpiresIn = authResult.Value.RefreshTokenExpiresIn
        };
        await Send.OkAsync(Response, ct);
    }
}