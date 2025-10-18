using Common;

namespace Auth.Application.Interfaces;

public interface IUserManager
{
    Task<Result<Guid>> CreateUserAsync(
        string email,
        string password,
        string? firstName,
        string? lastName,
        string? middleName,
        string? phone,
        CancellationToken ct = default);

    Task<bool> UserExistsAsync(string email, CancellationToken ct = default);
    Task<Result<bool>> IsInRoleAsync(Guid userId, string role, CancellationToken ct = default);
    Task<Result<bool>> AddToRoleAsync(Guid userId, string role, CancellationToken ct = default);

    Task<bool> HasUsersAsync(CancellationToken ct = default);
}