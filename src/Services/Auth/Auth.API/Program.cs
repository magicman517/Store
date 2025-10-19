using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddFastEndpoints();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseAuthorization();

// app.UseFastEndpoints();

await app.RunAsync();