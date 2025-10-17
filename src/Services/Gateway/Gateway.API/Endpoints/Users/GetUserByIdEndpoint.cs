using FastEndpoints;

namespace Gateway.API.Endpoints.Users;

public class GetUserByIdEndpoint : Endpoint<Guid>
{
    public override void Configure()
    {
        Get("/users/{id:guid}");
        AllowAnonymous();

        Version(1);
        Description(b => b.Produces(200));
    }

    public override Task HandleAsync(Guid req, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}