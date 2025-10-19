using Common;

namespace Users.Core.Entities;

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

    public ICollection<string> Roles { get; set; } = [];
}