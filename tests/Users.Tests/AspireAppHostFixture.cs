using Aspire.Hosting;

namespace Users.Tests;

public class AspireAppHostFixture : IAsyncLifetime
{
    public required DistributedApplication App { get; set; }
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(120);

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>();

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        App = await appHost.BuildAsync();
        await App.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await App.StopAsync();
        await App.DisposeAsync();
    }
}