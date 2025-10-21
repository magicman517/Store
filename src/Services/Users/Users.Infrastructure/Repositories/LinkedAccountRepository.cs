using Microsoft.EntityFrameworkCore;
using Users.Core.Common;
using Users.Core.Entities;
using Users.Core.Repositories;
using Users.Infrastructure.Data;

namespace Users.Infrastructure.Repositories;

public class LinkedAccountRepository(UsersDbContext context) : ILinkedAccountRepository
{
    public async Task AddAsync(LinkedAccount linkedAccount, CancellationToken ct = default)
    {
        await context.LinkedAccounts.AddAsync(linkedAccount, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<LinkedAccount?> GetByProviderUserIdAsync(string providerUserId, CancellationToken ct = default)
    {
        return await context.LinkedAccounts.FirstOrDefaultAsync(la => la.ProviderUserId == providerUserId, ct);
    }

    public async Task<LinkedAccount?> GetByProviderAndProviderUserIdAsync(OauthProvider provider, string providerUserId, CancellationToken ct = default)
    {
        return await context.LinkedAccounts.FirstOrDefaultAsync(la => la.Provider == provider && la.ProviderUserId == providerUserId, ct);
    }
}