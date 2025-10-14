using Auth.Application.Interfaces;
using Auth.Infrastructure.Data;
using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Services;

public class UserManager(UserManager<ApplicationUser> userManager) : IUserManager
{
    public async Task<Result<Guid>> CreateUserAsync(string email, string password, string? firstName, string? lastName, string? middleName,
        string? phone, CancellationToken ct = default)
    {
        var exists = await UserExistsAsync(email, ct);
        if (exists) return Result<Guid>.Fail("Користувач з таким email вже існує", 409);

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            NormalizedFirstName = firstName?.ToUpperInvariant(),
            LastName = lastName,
            NormalizedLastName = lastName?.ToUpperInvariant(),
            MiddleName = middleName,
            NormalizedMiddleName = middleName?.ToUpperInvariant(),
            PhoneNumber = phone,
            PhoneNumberConfirmed = !string.IsNullOrWhiteSpace(phone)
        };

        var result = await userManager.CreateAsync(user, password);
        return !result.Succeeded
            ? Result<Guid>.Fail("Внутрішня помилка сервера", 500)
            : Result<Guid>.Ok(user.Id);
    }

    public async Task<bool> UserExistsAsync(string email, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user is not null;
    }

    public async Task<Result<bool>> IsInRoleAsync(Guid userId, string role, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<bool>.Fail("Користувача не знайдено", 404);

        var isInRole = await userManager.IsInRoleAsync(user, role);
        return Result<bool>.Ok(isInRole);
    }

    public async Task<bool> HasUsersAsync(CancellationToken ct = default)
    {
        return await userManager.Users.AnyAsync(ct);
    }
}