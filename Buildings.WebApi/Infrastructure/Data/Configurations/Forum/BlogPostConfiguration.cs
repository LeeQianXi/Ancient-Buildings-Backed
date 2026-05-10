using Buildings.Infrastructure.Data.Entities.Forum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buildings.Infrastructure.Data.Configurations.Forum;

public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("BlogPost");
        builder.HasKey(e => e.Id)
            .HasName("PK_BlogPost_PostId");
        builder.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedNever();
        /* ---------- 字段 ---------- */
        builder.Property(e => e.AuthorId)
            .IsRequired()
            .ValueGeneratedNever();
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.IsAi)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(e => e.Tag);
        builder.Property(e => e.Data);
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
        builder.Property(e => e.Views);
        builder.Property(e => e.Likes);
        /* ---------- 索引 ---------- */
        builder.HasIndex(e => e.IsAi)
            .HasDatabaseName("IX_BlogPost_IsAi");
        builder.HasIndex(e => e.Tag)
            .HasDatabaseName("IX_BlogPost_Tag");
        // 时间排序/分页
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_BlogPost_CreatedAt");
        builder.HasIndex(e => e.UpdatedAt)
            .HasDatabaseName("IX_BlogPost_UpdateAt");
        /* ----------- 导航 -----------*/
        builder.HasOne(e => e.Author)
            .WithMany()
            .HasForeignKey(e => e.AuthorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        builder.HasMany(e => e.Comments)
            .WithOne(c => c.BlogPost)
            .HasForeignKey(e => e.PostId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}