using Auth.Infrastructure;
using FastEndpoints;
using Microsoft.AspNetCore.Localization;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var supportedCultures = new[] { "en", "uk" };
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture(supportedCultures[0]);
    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedCultures);

    options.RequestCultureProviders.Clear();
    options.RequestCultureProviders.Add(new CookieRequestCultureProvider
    {
        CookieName = "PARAGLIDE_LOCALE"
    });
    options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
});

builder.Services.AddFastEndpoints();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();

builder.Services.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/reference", options =>
    {
        options
            .WithTheme(ScalarTheme.Saturn)
            .HideModels()
            .HideClientButton()
            .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch);
    });
}

app.UseRequestLocalization();

app.UseFastEndpoints();

await app.RunAsync();