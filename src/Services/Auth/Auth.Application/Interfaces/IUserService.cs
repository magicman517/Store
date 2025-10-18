using Auth.Application.Dtos.User;
using Auth.Application.Dtos.User.Requests;
using Auth.Application.Dtos.User.Responses;
using Common;

namespace Auth.Application.Interfaces;

public interface IUserService
{
    Task<Result<Guid>> CreateUserAsync(CreateUserRequest dto, CancellationToken ct = default);
    Task<Result<UserDto>> GetUserByEmailAsync(string email, CancellationToken ct = default);
    Task<Result<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> IsPasswordValidAsync(UserDto userDto, string password, CancellationToken ct = default);
}