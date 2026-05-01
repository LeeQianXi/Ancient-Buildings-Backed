using Buildings.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Buildings.Migrations;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BuildingDbContext>
{
    public BuildingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BuildingDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=127.0.0.1;Database=buildings;User Id=sa;Password=!Q1w2e3r4;Encrypt=True;TrustServerCertificate=True;",
            option => option.MigrationsHistoryTable($"__EFMigrationsHistory_{nameof(Buildings)}"));
        return new BuildingDbContext(optionsBuilder.Options);
    }
}