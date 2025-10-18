using System.Data;
using Auth.Application.Dtos.User;
using Auth.Application.Dtos.User.Requests;
using Auth.Application.Dtos.User.Responses;
using Auth.Application.Interfaces;
using Common;
using Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Auth.Infrastructure.Services;

public class UserService(
    IUserManager userManager,
    IRoleManager roleManager,
    IStringLocalizer<UserService> localizer,
    AuthContext db) : IUserService
{
    public async Task<Result<Guid>> CreateUserAsync(CreateUserRequest dto, CancellationToken ct = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
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

            var createRoleResult = await roleManager.CreateRoleAsync(roleToAssign, ct);
            if (!createRoleResult.IsSuccess)
            {
                await transaction.RollbackAsync(ct);
                return Result<Guid>.Fail(createRoleResult.Error, createRoleResult.StatusCode);
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
            try
            {
                await transaction.RollbackAsync(ct);
            }
            catch
            {
                /* swallow rollback errors */
            }

            return Result<Guid>.Fail(localizer["Error.Internal"], 500);
        }
    }

    public async Task<Result<UserDto>> GetUserByEmailAsync(string email, CancellationToken ct = default)
    {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user is null)
            return Result<UserDto>.Fail(localizer["Error.User.NotFound"], 404);

        var rolesResult = await userManager.GetUserRolesAsync(user.Id, ct);
        if (!rolesResult.IsSuccess)
            return Result<UserDto>.Fail(rolesResult.Error, rolesResult.StatusCode);

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Roles = rolesResult.Value.ToArray()
        };

        return Result<UserDto>.Ok(userDto);
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
            return Result<UserDto>.Fail(localizer["Error.User.NotFound"], 404);

        var rolesResult = await userManager.GetUserRolesAsync(user.Id, ct);
        if (!rolesResult.IsSuccess)
            return Result<UserDto>.Fail(rolesResult.Error, rolesResult.StatusCode);

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Roles = rolesResult.Value.ToArray()
        };

        return Result<UserDto>.Ok(userDto);
    }

    public async Task<bool> IsPasswordValidAsync(UserDto userDto, string password, CancellationToken ct = default)
    {
        return await userManager.IsPasswordValidAsync(userDto.Id, password, ct);
    }
}