using Auth.Application.Dtos.User.Requests;
using Common;

namespace Auth.Application.Interfaces;

public interface IUserService
{
    Task<Result<Guid>> CreateUserAsync(CreateUserRequest dto, CancellationToken ct = default);
}