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
    }
}