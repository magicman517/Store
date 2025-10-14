using Auth.Application.Interfaces;
using Auth.Infrastructure.Data;
using Common;
using Microsoft.AspNetCore.Identity;

namespace Auth.Infrastructure.Services;

public class RoleManager(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager) : IRoleManager
{
    public async Task<Result<bool>> CreateRoleAsync(string role, CancellationToken ct = default)
    {
        var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        return result.Succeeded
            ? Result<bool>.Ok(true)
            : Result<bool>.Fail("Внутрішня помилка сервера", 500);
    }

    public async Task<bool> RoleExistsAsync(string role, CancellationToken ct = default)
    {
        return await roleManager.RoleExistsAsync(role);
    }

    public async Task<Result<bool>> AddToRoleAsync(Guid userId, string role, CancellationToken ct = default)
    {
        var roleExists = await RoleExistsAsync(role, ct);
        if (!roleExists)
            await CreateRoleAsync(role, ct);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<bool>.Fail("Користувача не знайдено", 404);

        var result = await userManager.AddToRoleAsync(user, role);
        return result.Succeeded
            ? Result<bool>.Ok(true)
            : Result<bool>.Fail("Внутрішня помилка сервера", 500);
    }
}