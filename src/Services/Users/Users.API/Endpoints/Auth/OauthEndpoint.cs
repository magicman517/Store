using FastEndpoints;
using Users.Application.DTOs.Auth;
using Users.Core.Services;

namespace Users.API.Endpoints.Auth;

public class OauthEndpoint(IAuthService authService) : Endpoint<OauthRequest, TokenResponse>
{
    public override void Configure()
    {
        Post("/auth/oauth");
        AllowAnonymous();
    }

    public override async Task HandleAsync(OauthRequest req, CancellationToken ct)
    {
        var authResult = await authService.LoginWithOauthAsync(req.Provider, req.Email, req.ProviderUserId, ct);
        if (authResult.IsFailure)
        {
            ThrowError(authResult.Error, authResult.StatusCode);
        }

        await Send.OkAsync(TokenResponse.FromDto(authResult.Value), ct);
    }
}