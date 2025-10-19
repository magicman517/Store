using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Users.Application;
using Users.Infrastructure;
using Users.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var supportedCultures = new[] { "en", "uk" };
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture(supportedCultures[0]);
    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedCultures);
});

builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer(
        serviceName: "Keycloak",
        realm: "store",
        options =>
        {
            options.Audience = "account";
            // options.Authority = "http://localhost:8080/auth/realms/store";
            if (builder.Environment.IsDevelopment())
            {
                options.RequireHttpsMetadata = false;
            }
        });
builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// builder.Services.AddFastEndpoints();
builder.Services.AddOpenApi();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UsersContext>();
    await context.Database.MigrateAsync();
}

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

app.UseAuthentication();
app.UseAuthorization();

// app.UseFastEndpoints();

await app.RunAsync();