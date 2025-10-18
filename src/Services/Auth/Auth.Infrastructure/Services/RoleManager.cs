using Auth.Application.Interfaces;
using Common;
using Microsoft.AspNetCore.Identity;

namespace Auth.Infrastructure.Services;

public class RoleManager(RoleManager<IdentityRole<Guid>> roleManager) : IRoleManager
{
    public async Task<Result<bool>> CreateRoleAsync(string role, CancellationToken ct = default)
    {
        var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));

        if (result.Succeeded)
        {
            return Result<bool>.Ok(true);
        }

        var isDuplicateRole = result.Errors.Any(e =>
            e.Code.Equals("DuplicateRoleName", StringComparison.OrdinalIgnoreCase) ||
            e.Code.Equals("DuplicateName", StringComparison.OrdinalIgnoreCase));

        return isDuplicateRole
            ? Result<bool>.Ok(true)
            : Result<bool>.Fail("Internal server error", 500);
    }

    public async Task<bool> RoleExistsAsync(string role, CancellationToken ct = default)
    {
        return await roleManager.RoleExistsAsync(role);
    }
}