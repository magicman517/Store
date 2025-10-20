using Users.Core.Entities;

namespace Users.Core.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task DeleteAsync(RefreshToken refreshToken, CancellationToken ct = default);
}