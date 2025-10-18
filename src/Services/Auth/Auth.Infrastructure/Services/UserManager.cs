using Auth.Application.Interfaces;
using Auth.Infrastructure.Data;
using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Auth.Infrastructure.Services;

public class UserManager(UserManager<ApplicationUser> userManager, IStringLocalizer<UserManager> localizer) : IUserManager
{
    public async Task<Result<Guid>> CreateUserAsync(string email, string password, string? firstName, string? lastName, string? middleName,
        string? phone, CancellationToken ct = default)
    {
        var exists = await UserExistsAsync(email, ct);
        if (exists) return Result<Guid>.Fail(localizer["Error.Email.IsTaken"], 409);

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
            ? Result<Guid>.Fail(localizer["Error.Internal"], 500)
            : Result<Guid>.Ok(user.Id);
    }

    public async Task<Result<IEnumerable<string>>> GetUserRolesAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
                        return Result<IEnumerable<string>>.Fail(localizer["Error.User.NotFound"], 404);

        var roles = await userManager.GetRolesAsync(user);
        return Result<IEnumerable<string>>.Ok(roles);
    }

    public async Task<bool> IsPasswordValidAsync(Guid userId, string password, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return false;

        return await userManager.CheckPasswordAsync(user, password);
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
            return Result<bool>.Fail(localizer["Error.User.NotFound"], 404);

        var isInRole = await userManager.IsInRoleAsync(user, role);
        return Result<bool>.Ok(isInRole);
    }

    public async Task<Result<bool>> AddToRoleAsync(Guid userId, string role, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<bool>.Fail(localizer["Error.User.NotFound"], 404);

        var result = await userManager.AddToRoleAsync(user, role);
        return result.Succeeded
            ? Result<bool>.Ok(true)
            : Result<bool>.Fail(localizer["Error.Internal"], 500);
    }

    public async Task<bool> HasUsersAsync(CancellationToken ct = default)
    {
        return await userManager.Users.AnyAsync(ct);
    }
}