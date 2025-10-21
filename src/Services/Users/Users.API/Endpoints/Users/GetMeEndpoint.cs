using FastEndpoints;
using Users.Application.DTOs.Users;
using Users.Core.Services;

namespace Users.API.Endpoints.Users;

public class GetMeEndpoint(IUserService userService) : EndpointWithoutRequest<UserResponse>
{
    public override void Configure()
    {
        Get("/@me");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (userId is null)
        {
            ThrowError("Unauthorized", 401);
        }

        var userResult = await userService.GetByIdAsync(Guid.Parse(userId), ct);
        if (userResult.IsFailure)
        {
            ThrowError(userResult.Error, userResult.StatusCode);
        }

        await Send.OkAsync(UserResponse.FromEntity(userResult.Value), ct);
    }
}