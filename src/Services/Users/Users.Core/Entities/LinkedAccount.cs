using Common;
using Users.Core.Common;

namespace Users.Core.Entities;

public class LinkedAccount : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public OauthProvider Provider { get; set; }
    public required string ProviderUserId { get; set; }
}