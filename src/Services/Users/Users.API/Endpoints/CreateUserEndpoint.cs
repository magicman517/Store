using FastEndpoints;
using Users.Application.DTOs;
using Users.Core.Services;

namespace Users.API.Endpoints;

public class CreateUserEndpoint(IUserService userService) : Endpoint<CreateUserRequest>
{
    public override void Configure()
    {
        Post("/");
        // AllowAnonymous();

        Description(o =>
        {
            o.Produces(201);
        });
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

        await Send.CreatedAtAsync(string.Empty, cancellation: ct);
    }
}