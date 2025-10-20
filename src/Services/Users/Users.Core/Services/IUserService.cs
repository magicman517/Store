using Common;

namespace Users.Core.Services;

public interface IUserService
{
    Task<Result<Guid>> CreateUserAsync(
        string email,
        string? password,
        string? firstName = null,
        string? lastName = null,
        string? middleName = null,
        string? phoneNumber = null,
        CancellationToken ct = default);
}