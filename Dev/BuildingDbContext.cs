using Dev.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dev;

public class BuildingDbContext(DbContextOptions<BuildingDbContext> options) : DbContext(options)
{
    public DbSet<BuildingArticleData> BuildingArticleDatas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new BuildingArticleConfiguration());
    }
}