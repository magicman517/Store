using Common;
using Users.Core.Entities;
using Users.Core.Repositories;
using Users.Core.Services;

namespace Users.Infrastructure.Services;

public class UserService(IUserRepository userRepository, IHashingService hashingService) : IUserService
{
    private readonly IList<string> _defaultRoles = ["User"];

    public async Task<Result<Guid>> CreateUserAsync(string email, string? password, string? firstName = null, string? lastName = null,
        string? middleName = null, string? phoneNumber = null, CancellationToken ct = default)
    {
        // TODO localize responses
        var existingUser =  await userRepository.GetByEmailAsync(email, ct);
        if (existingUser is not null)
        {
            return Result<Guid>.Fail("User with this email already exists", 409);
        }

        var hashedPassword = password is not null ? hashingService.HashPassword(password) : null;

        var user = new User
        {
            Email = email,
            PasswordHash = hashedPassword,
            FirstName = firstName,
            LastName = lastName,
            MiddleName = middleName,
            PhoneNumber = phoneNumber,
            Roles = _defaultRoles
        };

        await userRepository.AddAsync(user, ct);
        return Result<Guid>.Ok(user.Id);
    }
}