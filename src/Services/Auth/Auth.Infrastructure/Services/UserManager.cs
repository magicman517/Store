using Auth.Application.Interfaces;
using Auth.Infrastructure.Data;
using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Auth.Infrastructure.Services;

public class UserManager(UserManager<ApplicationUser> userManager, IStringLocalizer<UserManager> localizer) : IUserManager
{
    /// <summary>
    /// Creates a new user account with the given email, password, and optional profile information.
    /// </summary>
    /// <param name="phone">Optional phone number; if provided, the phone number is marked as confirmed.</param>
    /// <returns>
    /// A Result containing the new user's Id on success.
    /// On failure, the Result contains a localized error message and status code: 409 if the email is already taken, 500 for internal creation errors.
    /// </returns>
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

    /// <summary>
    /// Retrieves the role names assigned to the specified user.
    /// </summary>
    /// <param name="userId">The identifier of the user whose roles will be retrieved.</param>
    /// <returns>
    /// A Result containing the user's role names on success; a failed Result with a localized "Error.User.NotFound" message and HTTP 404 status if the user does not exist.
    /// </returns>
    public async Task<Result<IEnumerable<string>>> GetUserRolesAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
                        return Result<IEnumerable<string>>.Fail(localizer["Error.User.NotFound"], 404);

        var roles = await userManager.GetRolesAsync(user);
        return Result<IEnumerable<string>>.Ok(roles);
    }

    /// <summary>
    /// Determines whether the provided password matches the stored credentials for the specified user.
    /// </summary>
    /// <param name="userId">The identifier of the user whose password should be validated.</param>
    /// <param name="password">The plaintext password to validate against the user's stored password.</param>
    /// <returns>`true` if the password matches the user's stored credentials; `false` otherwise (including when the user does not exist).</returns>
    public async Task<bool> IsPasswordValidAsync(Guid userId, string password, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return false;

        return await userManager.CheckPasswordAsync(user, password);
    }

    /// <summary>
    /// Determines whether a user with the specified email exists.
    /// </summary>
    /// <param name="email">The email address to check for an existing user.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>`true` if a user with the specified email exists, `false` otherwise.</returns>
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