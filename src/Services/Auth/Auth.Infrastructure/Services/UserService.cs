using Auth.Application.Dtos.User.Requests;
using Auth.Application.Interfaces;
using Common;

namespace Auth.Infrastructure.Services;

public class UserService(IUserManager userManager, IRoleManager roleManager) : IUserService
{
    public async Task<Result<Guid>> CreateUserAsync(CreateUserRequest dto, CancellationToken ct = default)
    {
        var isFirstUser = !await userManager.HasUsersAsync(ct);
        var roleToAssign = isFirstUser ? "Admin" : "User";

        var createUserResult = await userManager.CreateUserAsync(
            dto.Email,
            dto.Password,
            dto.FirstName,
            dto.LastName,
            dto.MiddleName,
            dto.Phone,
            ct);

        if (!createUserResult.IsSuccess)
        {
            return Result<Guid>.Fail(createUserResult.Error, createUserResult.StatusCode);
        }

        var userId = createUserResult.Value;

        var roleExists = await roleManager.RoleExistsAsync(roleToAssign, ct);
        if (!roleExists)
        {
            var createRoleResult = await roleManager.CreateRoleAsync(roleToAssign, ct);
            if (!createRoleResult.IsSuccess)
            {
                return Result<Guid>.Fail(createRoleResult.Error, createRoleResult.StatusCode);
            }
        }

        var addToRoleResult = await userManager.AddToRoleAsync(userId, roleToAssign, ct);
        return !addToRoleResult.IsSuccess
            ? Result<Guid>.Fail(addToRoleResult.Error, addToRoleResult.StatusCode)
            : Result<Guid>.Ok(userId);
    }
}