using Auth.Application.Dtos.Auth.Requests;
using Auth.Application.Dtos.Auth.Responses;
using Auth.Application.Interfaces;
using FastEndpoints;

namespace Auth.API.Endpoints.Auth;

public class RefreshEndpoint(IAuthService authService) : Endpoint<RefreshRequest, TokenResponse>
{
    public override void Configure()
    {
        Post("auth/refresh");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RefreshRequest req, CancellationToken ct)
    {
        var refreshResult = await authService.RefreshAuthAsync(req.RefreshToken, ct);
        if (refreshResult.IsFailure)
        {
            ThrowError(refreshResult.Error, refreshResult.StatusCode);
        }

        Response = new TokenResponse
        {
            AccessToken = refreshResult.Value.AccessToken,
            AccessTokenExpiresIn = refreshResult.Value.AccessTokenExpiresIn,
            RefreshToken = refreshResult.Value.RefreshToken,
            RefreshTokenExpiresIn = refreshResult.Value.RefreshTokenExpiresIn
        };
        await Send.OkAsync(Response, ct);
    }
}