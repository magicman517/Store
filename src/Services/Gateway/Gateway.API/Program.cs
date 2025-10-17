using Aspire.ServiceDefaults;
using FastEndpoints;
using FastEndpoints.Security;
using Gateway.Application;
using Gateway.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = "development-signing-key-change-me-in-production");
builder.Services.AddAuthorization();

builder.Services.AddFastEndpoints();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(config =>
{
    config.Versioning.Prefix = "v";
    config.Versioning.PrependToRoute = true;
});

app.Run();