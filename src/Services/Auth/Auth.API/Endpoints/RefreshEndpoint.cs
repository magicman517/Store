using Auth.Application.DTOs;
using Auth.Core.Services;
using FastEndpoints;

namespace Auth.API.Endpoints;

public class RefreshEndpoint(IAuthService authService, IWebHostEnvironment environment) : Endpoint<RefreshAuthRequest>
{
    public override void Configure()
    {
        Post("/refresh");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RefreshAuthRequest req, CancellationToken ct)
    {
        var tokensResult = await authService.RefreshAuthAsync(req.RefreshToken, ct);
        if (tokensResult.IsFailure)
        {
            ThrowError(tokensResult.Error, tokensResult.StatusCode);
        }

        var tokensJson = tokensResult.Value;
        var useSecureCookies = environment.IsProduction();

        HttpContext.Response.Cookies.Append(
            "SECURE_access_token",
            tokensJson.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = useSecureCookies,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                MaxAge = TimeSpan.FromSeconds(tokensJson.ExpiresIn)
            });

        HttpContext.Response.Cookies.Append(
            "SECURE_refresh_token",
            tokensJson.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = useSecureCookies,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                MaxAge = TimeSpan.FromSeconds(tokensJson.RefreshExpiresIn)
            });

        await Send.NoContentAsync(ct);
    }
}