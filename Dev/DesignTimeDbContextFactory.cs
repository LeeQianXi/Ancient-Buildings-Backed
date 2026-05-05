using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Dev;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PgContent>
{
    public PgContent CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PgContent>();
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
        if (connectionString is null)
        {
            var connectionStringPath = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING_FILE")
                                       ?? throw new ArgumentNullException(nameof(connectionString));
            connectionString = File.ReadAllText(connectionStringPath);
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        optionsBuilder.UseSqlite(connectionString);
        return new PgContent(optionsBuilder.Options);
    }
}