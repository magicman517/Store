namespace Auth.Application.DTOs;

public record RefreshAuthRequest
{
    public required string RefreshToken { get; set; }
}