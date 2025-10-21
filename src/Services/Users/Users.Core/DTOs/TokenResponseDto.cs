namespace Users.Core.DTOs;

public record TokenResponseDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public int ExpiresIn { get; init; }
    public int RefreshExpiresIn { get; init; }
}