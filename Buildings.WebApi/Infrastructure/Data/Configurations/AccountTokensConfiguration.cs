using Buildings.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buildings.Infrastructure.Data.Configurations;

public class AccountTokensConfiguration : IEntityTypeConfiguration<AccountTokens>
{
    public void Configure(EntityTypeBuilder<AccountTokens> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("AccountTokens");

        builder.HasKey(e => new { e.UserId, e.Hash })
            .HasName("PK_AccountTokens_UserId");

        builder.Property(e => e.UserId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.Hash)
            .IsRequired()
            .ValueGeneratedNever();

        /* ---------- 字段 ---------- */
        builder.Property(e => e.RefreshToken);
        builder.Property(e => e.RefreshTokenExpiry);
    }
}