using Aspire.ServiceDefaults;
using Auth.Application;
using Auth.Infrastructure;
using Auth.Infrastructure.Data;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
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
});

builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = "development-signing-key-change-me-in-production");
builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddFastEndpoints();
builder.Services.AddOpenApi();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuthContext>();
    context.Database.Migrate();
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

app.UseFastEndpoints();

await app.RunAsync();