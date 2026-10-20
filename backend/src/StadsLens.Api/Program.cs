using StadsLens.Application;
using StadsLens.Infrastructure;
using StadsLens.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

LoadDotEnv(builder.Environment.ContentRootPath);
builder.Configuration.AddEnvironmentVariables();

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
        var dbContext = scope.ServiceProvider.GetRequiredService<StadsLensDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        await DbSeeder.SeedAsync(dbContext);
    }

    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "StadsLens API"));
}

// Geen HTTPS-redirect: Ngrok verzorgt HTTPS naar buiten toe.

app.UseAuthorization();

app.MapControllers();

app.Run();

static void LoadDotEnv(string contentRootPath)
{
    var path = Path.Combine(contentRootPath, ".env");
    if (!File.Exists(path))
        return;

    foreach (var line in File.ReadLines(path))
    {
        var trimmed = line.Trim();
        if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
            continue;

        var separator = trimmed.IndexOf('=');
        if (separator <= 0)
            continue;

        var name = trimmed[..separator].Trim();
        var value = trimmed[(separator + 1)..].Trim().Trim('"');
        Environment.SetEnvironmentVariable(name, value);
    }
}
