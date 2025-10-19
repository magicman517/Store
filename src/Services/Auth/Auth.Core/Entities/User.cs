using Common;

namespace Auth.Core.Entities;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public bool EmailConfirmed { get; set; }

    public required string PasswordHash { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }

    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }

    public HashSet<string> Roles { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}