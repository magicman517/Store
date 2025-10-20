using FastEndpoints;
using Users.Application.DTOs.Auth;
using Users.Core.Services;

namespace Users.API.Endpoints.Auth;

public class RefreshEndpoint(IAuthService authService) : Endpoint<RefreshRequest, TokenResponse>
{
    public override void Configure()
    {
        Post("/auth/refresh");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RefreshRequest req, CancellationToken ct)
    {
        var authResult = await authService.RefreshAuthAsync(req.RefreshToken, ct);
        if (authResult.IsFailure)
        {
            ThrowError(authResult.Error, authResult.StatusCode);
        }

        await Send.OkAsync(TokenResponse.FromDto(authResult.Value), ct);
    }
}