namespace Auth.Application.Dtos.Auth.Responses;

public class RefreshTokenDto
{
    public Guid Id { get; set; }

    public required string Token { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpiresAt { get; set; }

    public Guid UserId { get; set; }
}