using Common;

namespace Users.Core.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}