using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Users.Application;
using Users.Core.Entities;
using Users.Core.Repositories;
using Users.Core.Services;
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

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddFastEndpoints();
builder.Services.AddOpenApi();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<UsersDbContext>();
    await dbContext.Database.MigrateAsync();

    var adminEmail = builder.Configuration["Admin:Email"];
    var adminPassword = builder.Configuration["Admin:Password"];
    if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
    {
        var passwordHasher = services.GetRequiredService<IHashingService>();
        var usersRepository = services.GetRequiredService<IUserRepository>();
        var anyUserExist = await usersRepository.AnyUserExistsAsync();
        if (!anyUserExist)
        {
            var user = new User
            {
                Email = adminEmail,
                PasswordHash = passwordHasher.HashPassword(adminPassword),
                Roles = ["User", "Admin"]
            };
            await usersRepository.AddAsync(user);
        }
    }
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