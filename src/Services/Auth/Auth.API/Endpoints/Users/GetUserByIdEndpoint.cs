using FastEndpoints;

namespace Auth.API.Endpoints.Users;

public class GetUserByIdEndpoint : Endpoint<Guid>
{
    public override void Configure()
    {
        Get("/users/{id:guid}");

        Description(b => b.Produces(200));
    }

    public override Task HandleAsync(Guid req, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}