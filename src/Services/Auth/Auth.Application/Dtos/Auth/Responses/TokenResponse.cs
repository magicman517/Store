namespace Auth.Application.Dtos.Auth.Responses;

public record TokenResponse
{
    public required string AccessToken { get; set; }
    public int AccessTokenExpiresIn { get; set; } = (int)TimeSpan.FromMinutes(30).TotalSeconds;

    public required string RefreshToken { get; set; }
    public int RefreshTokenExpiresIn { get; set; } = (int)TimeSpan.FromDays(7).TotalSeconds;
}