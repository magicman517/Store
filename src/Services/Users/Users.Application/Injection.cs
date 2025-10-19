using Microsoft.Extensions.DependencyInjection;

namespace Users.Application;

public static class Injection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}