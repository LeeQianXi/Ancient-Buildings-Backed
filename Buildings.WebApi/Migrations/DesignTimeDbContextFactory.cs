using Buildings.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Buildings.Migrations;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BuildingDbContext>
{
    public BuildingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BuildingDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
        if (connectionString is null)
        {
            var connectionStringPath = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING_FILE")
                                       ?? throw new ArgumentNullException(nameof(connectionString));
            connectionString = File.ReadAllText(connectionStringPath);
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        optionsBuilder.UseNpgsql(connectionString,
            option => option.MigrationsHistoryTable($"__EFMigrationsHistory_{nameof(Buildings)}"));
        return new BuildingDbContext(optionsBuilder.Options);
    }
}