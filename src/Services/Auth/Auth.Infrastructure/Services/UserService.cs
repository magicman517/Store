using Auth.Application.Dtos.User.Requests;
using Auth.Application.Interfaces;
using Common;
using Auth.Infrastructure.Data;

namespace Auth.Infrastructure.Services;

public class UserService(IUserManager userManager, IRoleManager roleManager, AuthContext db) : IUserService
{
    public async Task<Result<Guid>> CreateUserAsync(CreateUserRequest dto, CancellationToken ct = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
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
                await transaction.RollbackAsync(ct);
                return Result<Guid>.Fail(createUserResult.Error, createUserResult.StatusCode);
            }

            var userId = createUserResult.Value;

            var roleExists = await roleManager.RoleExistsAsync(roleToAssign, ct);
            if (!roleExists)
            {
                var createRoleResult = await roleManager.CreateRoleAsync(roleToAssign, ct);
                if (!createRoleResult.IsSuccess)
                {
                    await transaction.RollbackAsync(ct);
                    return Result<Guid>.Fail(createRoleResult.Error, createRoleResult.StatusCode);
                }
            }

            var addToRoleResult = await userManager.AddToRoleAsync(userId, roleToAssign, ct);
            if (!addToRoleResult.IsSuccess)
            {
                await transaction.RollbackAsync(ct);
                return Result<Guid>.Fail(addToRoleResult.Error, addToRoleResult.StatusCode);
            }

            await transaction.CommitAsync(ct);
            return Result<Guid>.Ok(userId);
        }
        catch (OperationCanceledException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (Exception)
        {
            try { await transaction.RollbackAsync(ct); } catch { /* swallow rollback errors */ }
            return Result<Guid>.Fail("Internal server error", 500);
        }
    }
}