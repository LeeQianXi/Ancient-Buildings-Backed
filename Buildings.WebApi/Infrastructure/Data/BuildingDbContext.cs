using Microsoft.EntityFrameworkCore;

namespace Buildings.Infrastructure.Data;

public class BuildingDbContext(DbContextOptions<BuildingDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}