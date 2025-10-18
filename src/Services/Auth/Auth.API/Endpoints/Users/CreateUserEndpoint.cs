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

    /// <summary>
    /// Handles incoming requests to create a new user and returns a 201 Created response pointing to the newly created user.
    /// </summary>
    /// <param name="req">The request payload containing the new user's data.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var result = await userService.CreateUserAsync(req, ct);

        if (result.IsFailure)
        {
            ThrowError(result.Error, result.StatusCode);
        }

        await Send.CreatedAtAsync<GetUserByIdEndpoint>(new { Id = result.Value }, cancellation: ct);
    }
}