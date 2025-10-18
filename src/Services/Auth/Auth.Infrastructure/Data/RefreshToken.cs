namespace Auth.Infrastructure.Data;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public required string Token { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpiresAt { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
}