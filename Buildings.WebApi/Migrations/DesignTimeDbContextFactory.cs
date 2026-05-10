using Buildings.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Buildings.Migrations;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BuildingDbContext>
{
    public BuildingDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
        if (connectionString is null)
        {
            var path = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING_FILE");
            if (File.Exists(path)) connectionString = File.ReadAllText(path);
        }

        if (connectionString is null)
            throw new ArgumentNullException(nameof(connectionString));
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        var optionsBuilder = new DbContextOptionsBuilder<BuildingDbContext>();
        optionsBuilder.UseNpgsql(dataSource,
            option => option.MigrationsHistoryTable($"__EFMigrationsHistory_{nameof(Buildings)}"));
        return new BuildingDbContext(optionsBuilder.Options);
    }
}