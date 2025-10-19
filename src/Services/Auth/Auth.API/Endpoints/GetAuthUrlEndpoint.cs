using Auth.Core.DTOs;
using Auth.Core.Services;
using FastEndpoints;

namespace Auth.API.Endpoints;

public class GetAuthUrlEndpoint(IAuthService authService) : EndpointWithoutRequest<AuthUrlResponseDto>
{

    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Response = authService.GetAuthUrl();
        await Send.OkAsync(Response, cancellation: ct);
    }
}