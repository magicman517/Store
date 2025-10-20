using Common;
using Microsoft.Extensions.Localization;
using Users.Core.Entities;
using Users.Core.Repositories;
using Users.Core.Services;

namespace Users.Infrastructure.Services;

public class UserService(IStringLocalizer<UserService> localizer, IUserRepository userRepository, IHashingService hashingService) : IUserService
{
    private readonly IList<string> _defaultRoles = ["User"];

    public async Task<Result<Guid>> CreateUserAsync(string email, string? password, string? firstName = null, string? lastName = null,
        string? middleName = null, string? phoneNumber = null, CancellationToken ct = default)
    {
        var existingUser =  await userRepository.GetByEmailAsync(email, ct);
        if (existingUser is not null)
        {
            return Result<Guid>.Fail(localizer["Error.Email.IsTaken"], 409);
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