using Buildings.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buildings.Infrastructure.Data.Configurations;

public class FriendRelationConfiguration : IEntityTypeConfiguration<FriendRelation>
{
    public void Configure(EntityTypeBuilder<FriendRelation> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("FriendRelation");
        builder.HasKey(e => new { e.FromUserId, e.TargetUserId })
            .HasName("PK_FriendRelation_FromUserId_TargetUserId");

        builder.Property(e => e.FromUserId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.TargetUserId)
            .IsRequired()
            .ValueGeneratedNever();
        /* ---------- 字段 ---------- */
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
        builder.Property(e => e.DeleteAt)
            .ValueGeneratedNever();
        /* ---------- 索引 ---------- */
        builder.HasIndex(e => new { e.FromUserId, e.TargetUserId, e.DeleteAt })
            .HasDatabaseName("IX_FriendRelation_DeleteAt")
            .IsUnique();

        // 时间排序/分页
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_FriendRelation_CreatedAt");
    }
}