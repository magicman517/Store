using System.Data;
using Auth.Application.Dtos.User;
using Auth.Application.Dtos.User.Requests;
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
    /// <summary>
    /// Creates a new user and assigns a role ("Admin" for the first user, otherwise "User") within a serializable database transaction.
    /// </summary>
    /// <param name="dto">The request containing the new user's email, password, name parts, and phone number.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A Result containing the created user's Id on success; a failed Result with an error message and HTTP status code on failure.</returns>
    /// <exception cref="OperationCanceledException">Propagated when the operation is canceled via the provided cancellation token.</exception>
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

    /// <summary>
    /// Retrieves the user with the specified email and their assigned roles.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>A Result containing the user's UserDto with populated Roles if found; otherwise a failed Result with an error message and status code (404 if not found or other codes returned from role retrieval).</returns>
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

    /// <summary>
    /// Retrieves the user with the specified ID and returns a DTO that includes the user's assigned roles.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to retrieve.</param>
    /// <param name="ct">A cancellation token for the operation.</param>
    /// <returns>A Result&lt;UserDto&gt; containing the user DTO when found; `Fail` with status 404 and a localized message if the user is not found, or `Fail` with the role retrieval error and its status code if fetching roles fails.</returns>
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

    /// <summary>
    /// Verifies whether the provided password matches the specified user's credentials.
    /// </summary>
    /// <param name="userDto">The user whose password will be validated.</param>
    /// <param name="password">The plaintext password to validate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>`true` if the password is valid for the user, `false` otherwise.</returns>
    public async Task<bool> IsPasswordValidAsync(UserDto userDto, string password, CancellationToken ct = default)
    {
        return await userManager.IsPasswordValidAsync(userDto.Id, password, ct);
    }
}