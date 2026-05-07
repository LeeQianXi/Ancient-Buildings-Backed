using Dev.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dev;

public class ContentDbContent(DbContextOptions<ContentDbContent> options) : DbContext(options)
{
    public DbSet<ContentBuilding> ContentBuildings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ContentBuildingsConfiguration());
    }
}