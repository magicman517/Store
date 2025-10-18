using Auth.Application.Dtos.Auth.Requests;
using Auth.Application.Dtos.Auth.Responses;
using Auth.Application.Interfaces;
using FastEndpoints;

namespace Auth.API.Endpoints.Auth;

public class LoginEndpoint(IAuthService authService) : Endpoint<LoginRequest, TokenResponse>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

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