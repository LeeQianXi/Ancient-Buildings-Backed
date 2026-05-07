using Buildings.Infrastructure.Data.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buildings.Infrastructure.Data.Configurations.Account;

public class FriendRelationInfoConfiguration : IEntityTypeConfiguration<FriendRelationInfo>
{
    public void Configure(EntityTypeBuilder<FriendRelationInfo> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("FriendRelationInfo");
        builder.HasKey(e => e.Id)
            .HasName("PK_FriendRelationInfo_Id");
        builder.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedNever();
        /* ---------- 字段 ---------- */
        builder.Property(e => e.UserId)
            .IsRequired()
            .ValueGeneratedNever();
        builder.Property(e => e.FriendId)
            .IsRequired()
            .ValueGeneratedNever();
        builder.Property(e => e.Status)
            .HasConversion<short>()
            .HasDefaultValue(RequestStatus.Pending)
            .IsRequired();
        builder.Property(e => e.ActionUserId);
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate()
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
        /* ---------- 索引 ---------- */
        // 复合唯一约束：防止重复的关系对（User-Friend 唯一）
        builder.HasIndex(e => new { e.UserId, e.FriendId })
            .HasDatabaseName("IX_FriendRelationInfo_IdPair")
            .IsUnique();
        builder.HasIndex(e => new { e.UserId, e.Status })
            .HasDatabaseName("IX_FriendRelationInfo_UserId_Status");
        builder.HasIndex(e => new { e.FriendId, e.Status })
            .HasDatabaseName("IX_FriendRelationInfo_FriendId_Status");
        // 时间排序/分页
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_FriendRelationInfo_CreatedAt");
        /* ---------- 外键 ---------- */
        // 外键关系：User
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        // 外键关系：Friend
        builder.HasOne(e => e.Friend)
            .WithMany()
            .HasForeignKey(e => e.FriendId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}