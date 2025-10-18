using Auth.Application.Dtos.User;
using Auth.Application.Dtos.User.Requests;
using Common;

namespace Auth.Application.Interfaces;

public interface IUserService
{
    /// <summary>
/// Creates a new user from the provided request.
/// </summary>
/// <param name="dto">Data required to create the user.</param>
/// <returns>A Result containing the created user's GUID on success, or an error result.</returns>
Task<Result<Guid>> CreateUserAsync(CreateUserRequest dto, CancellationToken ct = default);
    /// <summary>
/// Retrieves a user by their email address.
/// </summary>
/// <param name="email">The email address of the user to retrieve.</param>
/// <param name="ct">Cancellation token to cancel the operation.</param>
/// <returns>A Result containing the matching <see cref="UserDto"/> if found, or an error result otherwise.</returns>
Task<Result<UserDto>> GetUserByEmailAsync(string email, CancellationToken ct = default);
    /// <summary>
/// Retrieves a user by their unique identifier.
/// </summary>
/// <param name="userId">The unique identifier of the user to retrieve.</param>
/// <returns>A Result containing the user's <see cref="UserDto"/> when found, or an error describing why retrieval failed.</returns>
Task<Result<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
    /// <summary>
/// Validates whether the provided password matches the specified user's stored credentials.
/// </summary>
/// <param name="userDto">The user data transfer object whose credentials will be validated.</param>
/// <param name="password">The plain-text password to verify against the user's stored password data.</param>
/// <returns>`true` if the password matches the user's credentials, `false` otherwise.</returns>
Task<bool> IsPasswordValidAsync(UserDto userDto, string password, CancellationToken ct = default);
}