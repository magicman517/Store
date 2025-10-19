using Auth.Core.Services;
using FastEndpoints;
using Microsoft.Extensions.Localization;

namespace Auth.API.Endpoints;

public class CallbackEndpoint(IAuthService authService, IWebHostEnvironment environment, IStringLocalizer<CallbackEndpoint> localizer) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/callback");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            var code = Query<string>("code");
            if (string.IsNullOrEmpty(code))
            {
                var redirectUrl = $"/auth?error={Uri.EscapeDataString(localizer["Error.Code.Missing"])}";
                await Send.RedirectAsync(redirectUrl);
                return;
            }

            var tokensResult = await authService.ExchangeCodeAsync(code, ct);
            if (tokensResult.IsFailure)
            {
                var redirectUrl = $"/auth?error={Uri.EscapeDataString(tokensResult.Error)}";
                await Send.RedirectAsync(redirectUrl);
                return;
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

            await Send.RedirectAsync("/");
        }
        catch (Exception)
        {
            var redirectUrl = $"/auth?error={Uri.EscapeDataString(localizer["Error.Internal"])}";
            await Send.RedirectAsync(redirectUrl);
        }
    }
}