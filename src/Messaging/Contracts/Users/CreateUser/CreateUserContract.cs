namespace Messaging.Contracts.Users.CreateUser;

public record CreateUserContractRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }
    public string? Phone { get; init; }
}

public record CreateUserContractResponse
{
    public Guid Id { get; init; }
}