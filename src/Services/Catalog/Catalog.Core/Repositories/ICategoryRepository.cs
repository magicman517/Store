using Catalog.Core.Entities;

namespace Catalog.Core.Repositories;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken ct = default);
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default);
    Task UpdateAsync(Category category, CancellationToken ct = default);
    Task DeleteAsync(Category category, CancellationToken ct = default);
}