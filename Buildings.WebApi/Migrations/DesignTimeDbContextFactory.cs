using Buildings.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Buildings.Migrations;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BuildingDbContext>
{
    public BuildingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BuildingDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING");
        if (connectionString is null)
        {
            var path = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING_FILE");
            if (File.Exists(path)) connectionString = File.ReadAllText(path);
        }

        if (connectionString is null)
            throw new ArgumentNullException(nameof(connectionString));
        optionsBuilder.UseSqlServer(connectionString,
            option => option.MigrationsHistoryTable($"__EFMigrationsHistory_{nameof(Buildings)}"));
        return new BuildingDbContext(optionsBuilder.Options);
    }
}