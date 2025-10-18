using Common;

namespace Auth.Application.Interfaces;

public interface IUserManager
{
    /// <summary>
        /// Creates a new user with the specified email, password, and optional personal details.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The password to set for the user.</param>
        /// <param name="firstName">Optional given name of the user.</param>
        /// <param name="lastName">Optional family name of the user.</param>
        /// <param name="middleName">Optional middle name of the user.</param>
        /// <param name="phone">Optional phone number for the user.</param>
        /// <returns>A Result containing the new user's Guid on success, or a Result carrying error information on failure.</returns>
        Task<Result<Guid>> CreateUserAsync(
        string email,
        string password,
        string? firstName,
        string? lastName,
        string? middleName,
        string? phone,
        CancellationToken ct = default);
    /// <summary>
/// Retrieves the role names assigned to the specified user.
/// </summary>
/// <param name="userId">The unique identifier of the user whose roles are requested.</param>
/// <returns>A Result containing the collection of role names on success, or an error describing the failure.</returns>
Task<Result<IEnumerable<string>>> GetUserRolesAsync(Guid userId, CancellationToken ct = default);
    /// <summary>
/// Determines whether the specified password is valid for the given user.
/// </summary>
/// <param name="userId">The identifier of the user to validate against.</param>
/// <param name="password">The plaintext password to validate.</param>
/// <param name="ct">Optional cancellation token.</param>
/// <returns>`true` if the provided password matches the user's credentials, `false` otherwise.</returns>
Task<bool> IsPasswordValidAsync(Guid userId, string password, CancellationToken ct = default);

    /// <summary>
/// Determines whether a user with the specified email exists.
/// </summary>
/// <param name="email">The email address to check for an existing user.</param>
/// <returns><c>true</c> if a user with the given email exists, <c>false</c> otherwise.</returns>
Task<bool> UserExistsAsync(string email, CancellationToken ct = default);
    /// <summary>
/// Determines whether the specified user belongs to the given role.
/// </summary>
/// <param name="userId">The identifier of the user to check.</param>
/// <param name="role">The name of the role to check membership for.</param>
/// <returns>`Result<bool>` containing `true` if the user is in the role, `false` otherwise; the `Result` contains an error on failure.</returns>
Task<Result<bool>> IsInRoleAsync(Guid userId, string role, CancellationToken ct = default);
    Task<Result<bool>> AddToRoleAsync(Guid userId, string role, CancellationToken ct = default);

    Task<bool> HasUsersAsync(CancellationToken ct = default);
}