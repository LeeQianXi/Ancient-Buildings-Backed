using Buildings.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buildings.Infrastructure.Data.Configurations;

public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("UserInfo");
        builder.HasKey(e => e.UserId)
            .HasName("PK_UserInfo_UserId");

        builder.Property(e => e.UserId)
            .IsRequired()
            .ValueGeneratedNever();
        /* ---------- 字段 ---------- */
        builder.Property(e => e.UserName)
            .IsRequired();
        builder.Property(e => e.Description);
        builder.Property(e => e.Online);
        builder.Property(e => e.Location);
        builder.Property(e => e.Avatar);
        builder.Property(e => e.Tags)
            .HasColumnType("jsonb");
        /* ---------- 索引 ---------- */
        builder.HasIndex(e => e.Tags)
            .HasDatabaseName("IX_UserInfo_Tags")
            .HasMethod("GIN");
        builder.HasIndex(e => e.Location)
            .HasDatabaseName("IX_UserInfo_Location");
    }
}