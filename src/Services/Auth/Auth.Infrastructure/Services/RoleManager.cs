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
            : Result<bool>.Fail("Internal server error", 500);
    }

    public async Task<bool> RoleExistsAsync(string role, CancellationToken ct = default)
    {
        return await roleManager.RoleExistsAsync(role);
    }
}