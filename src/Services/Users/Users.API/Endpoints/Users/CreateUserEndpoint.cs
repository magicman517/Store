using FastEndpoints;
using Users.Application.DTOs.Users;
using Users.Core.Services;

namespace Users.API.Endpoints.Users;

public class CreateUserEndpoint(IUserService userService) : Endpoint<CreateUserRequest>
{
    public override void Configure()
    {
        Post("/");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var userResult = await userService.CreateUserAsync(
            req.Email,
            req.Password,
            req.FirstName,
            req.LastName,
            req.MiddleName,
            req.Phone,
            ct);
        if (userResult.IsFailure)
        {
            ThrowError(userResult.Error, userResult.StatusCode);
        }

        await Send.OkAsync(new { UserId = userResult.Value }, ct);
    }
}