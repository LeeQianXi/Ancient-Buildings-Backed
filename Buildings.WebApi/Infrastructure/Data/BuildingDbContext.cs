using Buildings.Infrastructure.Data.Configurations;
using Buildings.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Buildings.Infrastructure.Data;

public class BuildingDbContext(DbContextOptions<BuildingDbContext> options) : DbContext(options)
{
    public DbSet<AccountUser> AccountUsers { get; set; }
    public DbSet<AccountTokens> AccountTokens { get; set; }
    public DbSet<BuildingArticleData> BuildingArticleData { get; set; }
    public DbSet<UserInfo> UserInfos { get; set; }
    public DbSet<FriendRequest> FriendRequests { get; set; }
    public DbSet<FriendRelation> FriendRelations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new AccountUserConfiguration())
            .ApplyConfiguration(new AccountTokensConfiguration())
            .ApplyConfiguration(new BuildingArticleConfiguration())
            .ApplyConfiguration(new UserInfoConfiguration())
            .ApplyConfiguration(new FriendRequestConfiguration())
            .ApplyConfiguration(new FriendRelationConfiguration());
    }
}