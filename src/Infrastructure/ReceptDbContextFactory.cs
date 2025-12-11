using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ReceptServer.Infrastructure;

public sealed class ReceptDbContextFactory : IDesignTimeDbContextFactory<ReceptDbContext>
{
    public ReceptDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReceptDbContext>();
        // Simpel SQLite connection til migrations og tooling.
        optionsBuilder.UseSqlite(BuildConnectionString());
        return new ReceptDbContext(optionsBuilder.Options);
    }

    private static string BuildConnectionString()
    {
        var rootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var defaultPath = Path.Combine(rootPath, "recept.db");
        var builder = new SqliteConnectionStringBuilder { DataSource = defaultPath };
        return builder.ToString();
    }
}
