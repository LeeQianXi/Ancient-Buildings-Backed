using Buildings.Infrastructure.Data.Entities.Secure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buildings.Infrastructure.Data.Configurations.Secure;

public class UserSecureInfoConfiguration : IEntityTypeConfiguration<UserSecureInfo>
{
    public void Configure(EntityTypeBuilder<UserSecureInfo> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("UserSecureInfo");
        builder.HasKey(e => e.UserId)
            .HasName("PK_UserSecureInfo_UserId");
        builder.Property(e => e.UserId)
            .IsRequired()
            .ValueGeneratedNever();
        /* ---------- 字段 ---------- */
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.PasswordSaltHash)
            .IsRequired()
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.UserName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate()
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
        builder.Property(e => e.DeleteAt)
            .ValueGeneratedNever();
        /* ---------- 索引 ---------- */
        builder.HasIndex(e => new { e.Email, e.DeleteAt })
            .HasDatabaseName("IX_UserSecureInfo_Email")
            .IsUnique();
        // 时间排序/分页
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_UserSecureInfo_CreatedAt");
        builder.HasIndex(e => e.UpdatedAt)
            .HasDatabaseName("IX_UserSecureInfo_UpdateAt");
    }
}