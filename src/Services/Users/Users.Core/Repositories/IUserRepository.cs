using Users.Core.Entities;

namespace Users.Core.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);

    Task AddToRoleAsync(User user, string role, CancellationToken ct = default);
    Task RemoveFromRoleAsync(User user, string role, CancellationToken ct = default);
}