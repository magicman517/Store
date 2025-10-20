using FastEndpoints;
using Users.Application.DTOs.Auth;
using Users.Core.Services;

namespace Users.API.Endpoints.Auth;

public class LoginEndpoint(IAuthService authService)
    : Endpoint<LoginRequest, TokenResponse>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var authResult = await authService.LoginWithPasswordAsync(req.Email, req.Password, ct);
        if (authResult.IsFailure)
        {
            ThrowError(authResult.Error, authResult.StatusCode);
        }

        await Send.OkAsync(TokenResponse.FromDto(authResult.Value), ct);
    }
}