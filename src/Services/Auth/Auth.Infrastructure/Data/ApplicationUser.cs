using Microsoft.AspNetCore.Identity;

namespace Auth.Infrastructure.Data;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? NormalizedFirstName { get; set; }

    public string? LastName { get; set; }
    public string? NormalizedLastName { get; set; }

    public string? MiddleName { get; set; }
    public string? NormalizedMiddleName { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}