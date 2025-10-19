using Auth.Core.DTOs;
using Auth.Core.Services;
using FastEndpoints;
using Microsoft.Extensions.Logging; 

namespace Auth.API.Endpoints;

public class GetAuthUrlEndpoint(IAuthService authService, ILogger<GetAuthUrlEndpoint> logger) : EndpointWithoutRequest
{

    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        foreach (var header in HttpContext.Request.Headers)
        {
            logger.LogDebug("Header: {Key}: {Value}", header.Key, header.Value);
        }
        logger.LogDebug("--- End Request Headers ---");

        var url = authService.GetAuthUrl();
        HttpContext.Response.Headers.Append("Location", url.AuthUrl);
        await Send.NoContentAsync(ct);
    }
}