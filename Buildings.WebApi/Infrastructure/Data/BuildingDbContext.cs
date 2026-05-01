using Buildings.Infrastructure.Data.Configurations;
using Buildings.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Buildings.Infrastructure.Data;

public class BuildingDbContext(DbContextOptions<BuildingDbContext> options) : DbContext(options)
{
    public DbSet<AccountUser> AccountUsers { get; set; }
    public DbSet<AccountTokens> AccountTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new AccountUserConfiguration());
        modelBuilder.ApplyConfiguration(new AccountTokensConfiguration());
    }
}