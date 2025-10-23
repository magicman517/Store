using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

public static class Injection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}