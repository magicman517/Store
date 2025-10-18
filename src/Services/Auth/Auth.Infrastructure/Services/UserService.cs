using System.Data;
using Auth.Application.Dtos.User;
using Auth.Application.Dtos.User.Requests;
using Auth.Application.Dtos.User.Responses;
using Auth.Application.Interfaces;
using Common;
using Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.Services;

public class UserService(
    IUserManager userManager,
    IRoleManager roleManager,
    IStringLocalizer<UserService> localizer,
    ILogger<UserService> logger,
    AuthContext db) : IUserService
{
    private static string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2)
            return "***";
        
        var local = parts[0];
        var domain = parts[1];
        
        if (local.Length <= 2)
            return $"{local[0]}***@{domain}";
        
        return $"{local[0]}***{local[^1]}@{domain}";
    }

    public async Task<Result<Guid>> CreateUserAsync(CreateUserRequest dto, CancellationToken ct = default)
    {
        logger.LogInformation("User registration attempt for email: {Email}", MaskEmail(dto.Email));
        
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
                logger.LogWarning("User registration failed for email: {Email} - {Error}", MaskEmail(dto.Email), createUserResult.Error);
                await transaction.RollbackAsync(ct);
                return Result<Guid>.Fail(createUserResult.Error, createUserResult.StatusCode);
            }

            var userId = createUserResult.Value;

            var createRoleResult = await roleManager.CreateRoleAsync(roleToAssign, ct);
            if (!createRoleResult.IsSuccess)
            {
                logger.LogWarning("User registration failed for email: {Email} - Role creation failed: {Error}", MaskEmail(dto.Email), createRoleResult.Error);
                await transaction.RollbackAsync(ct);
                return Result<Guid>.Fail(createRoleResult.Error, createRoleResult.StatusCode);
            }

            var addToRoleResult = await userManager.AddToRoleAsync(userId, roleToAssign, ct);
            if (!addToRoleResult.IsSuccess)
            {
                logger.LogWarning("User registration failed for email: {Email} - Role assignment failed: {Error}", MaskEmail(dto.Email), addToRoleResult.Error);
                await transaction.RollbackAsync(ct);
                return Result<Guid>.Fail(addToRoleResult.Error, addToRoleResult.StatusCode);
            }

            await transaction.CommitAsync(ct);
            logger.LogInformation("User registration successful for email: {Email} with role: {Role}", MaskEmail(dto.Email), roleToAssign);
            return Result<Guid>.Ok(userId);
        }
        catch (OperationCanceledException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "User registration failed for email: {Email} - Unexpected error", MaskEmail(dto.Email));
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