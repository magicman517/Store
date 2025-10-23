using Catalog.Application;
using Catalog.Infrastructure;
using FastEndpoints;
using FastEndpoints.Security;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddMinioClient("MinIO");

builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["Jwt:SigningKey"]);
builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddFastEndpoints();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();

app.Run();