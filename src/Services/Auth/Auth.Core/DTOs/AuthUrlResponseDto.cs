namespace Auth.Core.DTOs;

public record AuthUrlResponseDto
{
    public required string AuthUrl { get; set; }
}