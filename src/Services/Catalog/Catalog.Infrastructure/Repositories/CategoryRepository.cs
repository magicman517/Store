using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class CategoryRepository(CatalogDbContext context) : ICategoryRepository
{
    public async Task AddAsync(Category category, CancellationToken ct = default)
    {
        await context.Categories.AddAsync(category, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Categories.ToListAsync(ct);
    }

    public async Task UpdateAsync(Category category, CancellationToken ct = default)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Category category, CancellationToken ct = default)
    {
        context.Categories.Remove(category);
        await context.SaveChangesAsync(ct);
    }
}