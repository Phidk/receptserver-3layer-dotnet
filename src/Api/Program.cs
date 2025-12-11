using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using ReceptServer.Application;
using ReceptServer.Infrastructure;
using ReceptServer.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Infrastruktur wiring: SQLite DbContext + repository + service.
var connectionString = ResolveConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddDbContext<ReceptDbContext>(options =>
{
    options.UseSqlite(connectionString);
});
builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<PrescriptionService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/", () => Results.Text("Receptserver API skeleton running"));
app.MapControllers();

// Migrer DB ved opstart så dev/test ikke mangler tabeller.
await EnsureDatabaseAsync(app.Services);

app.Run();

static string ResolveConnectionString(string? configured)
{
    // Bruger konfigureret connection string, men løfter relative stier til en absolut sti i repo-roden.
    var rootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
    var defaultPath = Path.Combine(rootPath, "recept.db");

    if (string.IsNullOrWhiteSpace(configured))
    {
        return $"Data Source={defaultPath}";
    }

    var builder = new SqliteConnectionStringBuilder(configured);
    if (!Path.IsPathRooted(builder.DataSource))
    {
        builder.DataSource = defaultPath;
    }

    return builder.ToString();
}

static async Task EnsureDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ReceptDbContext>();
    await db.Database.MigrateAsync();
}
