namespace Users.Application.DTOs.Auth;

public record RefreshRequest
{
    public required string RefreshToken { get; init; }
}