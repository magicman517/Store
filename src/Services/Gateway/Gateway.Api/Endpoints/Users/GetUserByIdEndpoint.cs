using FastEndpoints;

namespace Gateway.Api.Endpoints.Users;

public class GetUserByIdEndpoint : Endpoint<Guid>
{
    public override void Configure()
    {
        Get("/users/{id:guid}");
        AllowAnonymous();

        Version(1);
        Description(b => b.Produces(200));
    }

    public override async Task HandleAsync(Guid req, CancellationToken ct)
    {

    }
}