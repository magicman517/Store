using Aspire.Hosting;

namespace Store.Tests;

public class AspireAppHostFixture : IAsyncLifetime
{
    public DistributedApplication? App { get; private set; }
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(120);

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Aspire_AppHost>();

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        App = await appHost.BuildAsync();
        await App.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (App is not null)
        {
            await App.StopAsync();
            await App.DisposeAsync();
        }
    }
}