using Common;

namespace Users.Core.Services;

public interface IUserService
{
    Task<Result<Guid>> CreateUserAsync(
        string email,
        string password,
        string? firstName,
        string? lastName,
        string? middleName,
        string? phoneNumber,
        CancellationToken ct = default);

    Task<bool> IsEmailTakenAsync(string email, CancellationToken ct = default);
}