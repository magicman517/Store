namespace Auth.Application.Dtos.Auth.Requests;

public record RefreshRequest
{
    public required string RefreshToken { get; init; }
}