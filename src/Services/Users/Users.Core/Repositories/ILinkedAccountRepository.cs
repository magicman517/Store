using Users.Core.Common;
using Users.Core.Entities;

namespace Users.Core.Repositories;

public interface ILinkedAccountRepository
{
    Task AddAsync(LinkedAccount linkedAccount, CancellationToken ct = default);
    Task<LinkedAccount?> GetByProviderUserIdAsync(string providerUserId, CancellationToken ct = default);
    Task<LinkedAccount?> GetByProviderAndProviderUserIdAsync(OauthProvider provider, string providerUserId, CancellationToken ct = default);
}