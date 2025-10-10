using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Guid> CreateUserAsync(
        string email,
        string password,
        string? firstName,
        string? lastName,
        string? middleName,
        string? phone,
        CancellationToken cancellationToken = default);

    Task AddRoleAsync(string role, CancellationToken cancellationToken = default);
    Task AddToRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> HasUsersAsync(CancellationToken cancellationToken = default);

    Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);
    Task<bool> RoleExistsAsync(string role, CancellationToken cancellationToken = default);
}