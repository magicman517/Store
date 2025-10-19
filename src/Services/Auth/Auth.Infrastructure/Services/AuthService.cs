using System.Text.Json;
using Auth.Core.DTOs;
using Auth.Core.Services;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.Services;

public class AuthService(
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    IStringLocalizer<AuthService> localizer,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly string _keycloakEndpoint =
        configuration.GetValue<string>("Keycloak:Url") 
        ?? throw new InvalidOperationException("Keycloak:Url configuration is required");

    private readonly string _redirectUrl = configuration.GetValue<string>("Keycloak:RedirectUrl")
        ?? throw new InvalidOperationException("Keycloak:RedirectUrl configuration is required");

    private readonly string _clientId = configuration.GetValue<string>("Keycloak:ClientId")
        ?? throw new InvalidOperationException("Keycloak:ClientId configuration is required");

    private readonly string _clientSecret = configuration.GetValue<string>("Keycloak:ClientSecret")
        ?? throw new InvalidOperationException("Keycloak:ClientSecret configuration is required");

    private readonly string _realm = configuration.GetValue<string>("Keycloak:Realm")
        ?? throw new InvalidOperationException("Keycloak:Realm configuration is required");

    public AuthUrlResponseDto GetAuthUrl()
    {
        return new AuthUrlResponseDto
        {
            AuthUrl = $"{_keycloakEndpoint}/realms/{_realm}/protocol/openid-connect/auth" +
                      $"?client_id={_clientId}" +
                      $"&response_type=code" +
                      $"&scope=openid profile" +
                      $"&redirect_uri={Uri.EscapeDataString(_redirectUrl)}"
        };
    }

    public async Task<Result<TokensResponseDto>> ExchangeCodeAsync(string code, CancellationToken ct = default)
    {
        using var httpClient = httpClientFactory.CreateClient();
        var tokenEndpoint = $"{_keycloakEndpoint}/realms/{_realm}/protocol/openid-connect/token";

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "code", code },
            { "redirect_uri", _redirectUrl }
        };

        var requestContent = new FormUrlEncodedContent(parameters);
        var response = await httpClient.PostAsync(tokenEndpoint, requestContent, ct);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Token exchange failed with status code: {StatusCode}", response.StatusCode);
            return Result<TokensResponseDto>.Fail(localizer["Error.Auth.Internal"], 500);
        }

        var responseContent = await response.Content.ReadAsStringAsync(ct);
        var tokens = JsonSerializer.Deserialize<TokensResponseDto>(responseContent);
        if (tokens != null)
        {
            return Result<TokensResponseDto>.Ok(tokens);
        }

        logger.LogError("Failed to deserialize token response: {ResponseContent}", responseContent);
        return Result<TokensResponseDto>.Fail(localizer["Error.Auth.Internal"], 500);
    }

    public async Task<Result<TokensResponseDto>> RefreshAuthAsync(string refreshToken, CancellationToken ct = default)
    {
        using var httpClient = httpClientFactory.CreateClient();
        var tokenEndpoint = $"{_keycloakEndpoint}/realms/{_realm}/protocol/openid-connect/token";

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "refresh_token", refreshToken }
        };

        var requestContent = new FormUrlEncodedContent(parameters);
        var response = await httpClient.PostAsync(tokenEndpoint, requestContent, ct);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Token refresh failed with status code: {StatusCode}", response.StatusCode);
            return Result<TokensResponseDto>.Fail(localizer["Error.Refresh.Internal"], 500);
        }

        var responseContent = await response.Content.ReadAsStringAsync(ct);
        var tokens = JsonSerializer.Deserialize<TokensResponseDto>(responseContent);
        if (tokens != null)
        {
            return Result<TokensResponseDto>.Ok(tokens);
        }

        logger.LogError("Failed to deserialize token response: {ResponseContent}", responseContent);
        return Result<TokensResponseDto>.Fail(localizer["Error.Refresh.Internal"], 500);
    }
}