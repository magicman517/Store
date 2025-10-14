using Common;

namespace Auth.Application.Interfaces;

public interface IRoleManager
{
    Task<Result<bool>> CreateRoleAsync(string role, CancellationToken ct = default);
    Task<bool> RoleExistsAsync(string role, CancellationToken ct = default);

    Task<Result<bool>> AddToRoleAsync(Guid userId, string role, CancellationToken ct = default);
}