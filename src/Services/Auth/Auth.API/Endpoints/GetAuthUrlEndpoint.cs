using Auth.Core.Services;
using FastEndpoints;

namespace Auth.API.Endpoints;

public class GetAuthUrlEndpoint(IAuthService authService) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var url = authService.GetAuthUrl();
        HttpContext.Response.Headers.Append("Location", url.AuthUrl);
        await Send.NoContentAsync(ct);
    }
}