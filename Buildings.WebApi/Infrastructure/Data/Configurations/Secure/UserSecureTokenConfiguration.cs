using Buildings.Infrastructure.Data.Entities.Secure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buildings.Infrastructure.Data.Configurations.Secure;

public class UserSecureTokenConfiguration : IEntityTypeConfiguration<UserSecureToken>
{
    public void Configure(EntityTypeBuilder<UserSecureToken> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("UserSecureToken");

        builder.HasKey(e => new { e.UserId, e.Hash })
            .HasName("PK_UserSecureToken_UserId");

        builder.Property(e => e.UserId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.Hash)
            .IsRequired()
            .ValueGeneratedNever();
        /* ---------- 字段 ---------- */
        builder.Property(e => e.RefreshToken)
            .IsRequired();
        builder.Property(e => e.RefreshTokenExpiry)
            .IsRequired();
        builder.Property(e => e.LastAcquired);
    }
}