using Common;

namespace Users.Core.Entities;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public string? PasswordHash { get; set; }

    public IList<LinkedAccount> LinkedAccounts { get; set; } = [];

    public IList<string> Roles { get; set; } = [];

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }

    public string? PhoneNumber { get; set; }
}