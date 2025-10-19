using Common;
using Users.Core.Entities;
using Users.Core.Repositories;
using Users.Core.Services;

namespace Users.Infrastructure.Services;

public class UserService(IUserRepository userRepository, IPasswordHasher hasher) : IUserService
{
    public async Task<Result<Guid>> CreateUserAsync(string email, string password, string? firstName, string? lastName, string? middleName,
        string? phoneNumber, CancellationToken ct = default)
    {
        if (await IsEmailTakenAsync(email, ct))
        {
            return Result<Guid>.Fail("Email is already taken", 409);
        }

        var userEntity = new User
        {
            Email = email,
            PasswordHash = hasher.Hash(password),
            FirstName = firstName,
            LastName = lastName,
            MiddleName = middleName,
            PhoneNumber = phoneNumber
        };
        await userRepository.AddAsync(userEntity, ct);
        return Result<Guid>.Ok(userEntity.Id);
    }

    public async Task<bool> IsEmailTakenAsync(string email, CancellationToken ct = default)
    {
        return await userRepository.GetByEmailAsync(email, ct) is not null;
    }
}