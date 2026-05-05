using Buildings.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buildings.Infrastructure.Data.Configurations;

public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
{
    public void Configure(EntityTypeBuilder<FriendRequest> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("FriendRequest");
        builder.HasKey(e => new { e.FromUserId, e.TargetUserId })
            .HasName("PK_FriendRequest_FromUserId_TargetUserId");

        builder.Property(e => e.FromUserId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.TargetUserId)
            .IsRequired()
            .ValueGeneratedNever();
        /* ---------- 字段 ---------- */
        builder.Property(e => e.Description)
            .IsRequired()
            .ValueGeneratedNever();
        builder.Property(e => e.Status)
            .ValueGeneratedNever();
    }
}