namespace Auth.Application.Dtos.User.Responses;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public string[] Roles { get; set; } = [];
}