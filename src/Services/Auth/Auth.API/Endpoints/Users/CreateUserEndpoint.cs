using Auth.Application.Dtos.User.Requests;
using Auth.Application.Interfaces;
using FastEndpoints;

namespace Auth.API.Endpoints.Users;

public class CreateUserEndpoint(IUserService userService) : Endpoint<CreateUserRequest>
{
    public override void Configure()
    {
        Post("/users");
        AllowAnonymous();

        Description(b => b.Produces(201));
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var result = await userService.CreateUserAsync(req, ct);

        if (result.IsSuccess)
        {
            await Send.CreatedAtAsync<GetUserByIdEndpoint>(new { Id = result.Value }, cancellation: ct);
        }
        else
        {
            AddError(result.Error);
            await Send.ErrorsAsync(result.StatusCode, ct);
        }
    }
}