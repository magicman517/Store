using FastEndpoints;

namespace Catalog.API.Endpoints.Products;

public class SeedEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/products/seed");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {

    }
}