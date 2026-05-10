using Buildings.Infrastructure.Data.Configurations;
using Buildings.Infrastructure.Data.Configurations.Account;
using Buildings.Infrastructure.Data.Configurations.Forum;
using Buildings.Infrastructure.Data.Configurations.Secure;
using Buildings.Infrastructure.Data.Entities;
using Buildings.Infrastructure.Data.Entities.Account;
using Buildings.Infrastructure.Data.Entities.Forum;
using Buildings.Infrastructure.Data.Entities.Secure;
using Microsoft.EntityFrameworkCore;

namespace Buildings.Infrastructure.Data;

public class BuildingDbContext(DbContextOptions<BuildingDbContext> options) : DbContext(options)
{
    /// <summary>
    ///     用户安全信息
    /// </summary>
    public DbSet<UserSecureInfo> UserSecureInfos { get; set; }

    /// <summary>
    ///     账号用户Token存储
    /// </summary>
    public DbSet<UserSecureToken> UserSecureTokens { get; set; }

    /// <summary>
    ///     建筑文章存储
    /// </summary>
    public DbSet<BuildingArticleData> BuildingArticleDatas { get; set; }

    /// <summary>
    ///     用户账号信息
    /// </summary>
    public DbSet<UserAccountInfo> UserAccountInfos { get; set; }

    /// <summary>
    ///     好友关系
    /// </summary>
    public DbSet<FriendRelationInfo> FriendRelationInfos { get; set; }

    public DbSet<BlogComment> BlogComments { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserSecureInfoConfiguration())
            .ApplyConfiguration(new UserSecureTokenConfiguration())
            .ApplyConfiguration(new BuildingArticleConfiguration())
            .ApplyConfiguration(new BlogCommentConfiguration())
            .ApplyConfiguration(new BlogPostConfiguration())
            .ApplyConfiguration(new UserAccountInfoConfiguration())
            .ApplyConfiguration(new FriendRelationInfoConfiguration());
    }
}