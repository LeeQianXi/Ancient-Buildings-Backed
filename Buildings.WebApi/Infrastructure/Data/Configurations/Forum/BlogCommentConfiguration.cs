using Buildings.Infrastructure.Data.Entities.Forum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buildings.Infrastructure.Data.Configurations.Forum;

public class BlogCommentConfiguration : IEntityTypeConfiguration<BlogComment>
{
    public void Configure(EntityTypeBuilder<BlogComment> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("BlogComment");
        builder.HasKey(e => e.Id)
            .HasName("PK_BlogComment_PostId");
        builder.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedNever();
        /* ---------- 字段 ---------- */
        builder.Property(e => e.AuthorId)
            .IsRequired()
            .ValueGeneratedNever();
        builder.Property(e => e.PostId)
            .IsRequired()
            .ValueGeneratedNever();
        builder.Property(e => e.Data)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(e => e.IsAi)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
        builder.Property(e => e.DeleteAt)
            .ValueGeneratedNever();
        /* ---------- 索引 ---------- */
        builder.HasIndex(e => e.IsAi)
            .HasDatabaseName("IX_BlogComment_IsAi");
        // 时间排序/分页
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_BlogComment_CreatedAt");
        /* ----------- 导航 -----------*/
        builder.HasOne(e => e.Author)
            .WithMany()
            .HasForeignKey(e => e.AuthorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        builder.HasOne(e => e.BlogPost)
            .WithMany(e => e.Comments)
            .HasForeignKey(e => e.PostId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}