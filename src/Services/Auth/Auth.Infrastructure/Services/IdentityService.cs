using Auth.Application.Common.Interfaces;
using Auth.Infrastructure.Data;
using Common.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Services;

public class IdentityService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager) : IIdentityService
{
    public async Task<Guid> CreateUserAsync(string email, string password, string? firstName, string? lastName, string? middleName,
        string? phone, CancellationToken cancellationToken = default)
    {
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
        return !result.Succeeded ? throw new ValidationApiException("Не вдалося створити користувача") : user.Id;
    }

    public async Task AddRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        var exists = await RoleExistsAsync(role, cancellationToken);
        if (exists) return;

        var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        if (!result.Succeeded)
            throw new ValidationApiException($"Не вдалося створити роль {role}");
    }

    public async Task AddToRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            throw new ValidationApiException("Користувача не знайдено");

        var exists = await roleManager.RoleExistsAsync(role);
        if (!exists)
            throw new ValidationApiException($"Роль {role} не існує");

        var result = await userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
            throw new ValidationApiException($"Не вдалося додати користувача до ролі {role}");
    }

    public async Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user is not null;
    }

    public async Task<bool> HasUsersAsync(CancellationToken cancellationToken = default)
    {
        return await userManager.Users.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            throw new ValidationApiException("Користувача не знайдено");

        return await userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> RoleExistsAsync(string role, CancellationToken cancellationToken = default)
    {
        return await roleManager.RoleExistsAsync(role);
    }
}