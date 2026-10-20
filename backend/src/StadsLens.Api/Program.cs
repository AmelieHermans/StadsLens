using StadsLens.Application;
using StadsLens.Infrastructure;
using StadsLens.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("StadsLensDb")
    ?? throw new InvalidOperationException("Connection string 'StadsLensDb' ontbreekt.");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        await DbSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<StadsLensDbContext>());
    }

    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "StadsLens API"));
}

// Geen HTTPS-redirect: Ngrok verzorgt HTTPS naar buiten toe.

app.UseAuthorization();

app.MapControllers();

app.Run();
