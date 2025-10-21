using Users.Core.DTOs;

namespace Users.Application.DTOs.Auth;

public record TokenResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public int ExpiresIn { get; init; }
    public int RefreshExpiresIn { get; init; }

    public static TokenResponse FromDto(TokenResponseDto dto) => new()
    {
        AccessToken = dto.AccessToken,
        RefreshToken = dto.RefreshToken,
        ExpiresIn = dto.ExpiresIn,
        RefreshExpiresIn = dto.RefreshExpiresIn
    };
}