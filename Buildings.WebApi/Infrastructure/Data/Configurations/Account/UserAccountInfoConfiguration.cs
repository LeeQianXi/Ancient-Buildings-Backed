using Buildings.Dtos;
using Buildings.Infrastructure.Data.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buildings.Infrastructure.Data.Configurations.Account;

public class UserAccountInfoConfiguration : IEntityTypeConfiguration<UserAccountInfo>
{
    public void Configure(EntityTypeBuilder<UserAccountInfo> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("UserAccountInfo");
        builder.HasKey(e => e.UserId)
            .HasName("PK_UserAccountInfo_UserId");
        builder.Property(e => e.UserId)
            .IsRequired()
            .ValueGeneratedNever();
        /* ---------- 字段 ---------- */
        builder.Property(e => e.UserName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Profile);
        builder.Property(e => e.Location);
        builder.Property(e => e.Gender)
            .HasConversion(
                v => v.Value,
                code => Gender.FromValue(code)
            )
            .HasDefaultValue(Gender.Unknown);
        builder.Property(e => e.Interest)
            .HasColumnType("jsonb");
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
        builder.HasIndex(e => new { e.UserName, e.DeleteAt })
            .HasDatabaseName("IX_UserAccountInfo_UserName");
        builder.HasIndex(e => e.Interest)
            .HasDatabaseName("IX_UserAccountInfo_Interest")
            .HasMethod("GIN");
        // 时间排序/分页
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_UserAccountInfo_CreatedAt");
        builder.HasIndex(e => e.UpdatedAt)
            .HasDatabaseName("IX_UserAccountInfo_UpdateAt");
    }
}