using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev.Entities;

public class ContentBuilding
{
    public required string Id { get; set; }

    public required string Title { get; set; }
    public required string Body { get; set; }
    public required string Categories { get; set; }
    public required string Desc { get; set; }
    public required string Description { get; set; }
    public required string Dynasties { get; set; }
    public required string Extension { get; set; }
    public required string Img { get; set; }
    public required string Meta { get; set; }
    public required string Name { get; set; }

    // 这里用 string 类型，默认值通过 Fluent API 设置
    public required string Navigation { get; set; }

    public required string Path { get; set; }
    public required string Provinces { get; set; }

    // seo 默认值 '{}'，在数据库中已有默认值，EF Core 中可忽略设置
    public required string Seo { get; set; }

    public required string Stem { get; set; }
    public required string Subtitle { get; set; }

    // 或在 Fluent API 中配置 HasIndex().IsUnique()
    public required string Hash { get; set; }
}

public class ContentBuildingsConfiguration : IEntityTypeConfiguration<ContentBuilding>
{
    public void Configure(EntityTypeBuilder<ContentBuilding> entity)
    {
        entity.ToTable("_content_buildings");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnType("TEXT");

        entity.Property(e => e.Title).HasColumnType("VARCHAR");
        entity.Property(e => e.Body).HasColumnType("TEXT");
        entity.Property(e => e.Categories).HasColumnType("TEXT");
        entity.Property(e => e.Desc).HasColumnType("VARCHAR");
        entity.Property(e => e.Description).HasColumnType("VARCHAR");
        entity.Property(e => e.Dynasties).HasColumnType("TEXT");
        entity.Property(e => e.Extension).HasColumnType("VARCHAR");
        entity.Property(e => e.Img).HasColumnType("VARCHAR");
        entity.Property(e => e.Meta).HasColumnType("TEXT");
        entity.Property(e => e.Name).HasColumnType("VARCHAR");

        // navigation 列：SQLite 中默认值用字符串 '1' 或 'true'，取决于你实际数据
        entity.Property(e => e.Navigation)
            .HasColumnType("TEXT")
            .HasDefaultValue("true");

        entity.Property(e => e.Path).HasColumnType("VARCHAR");
        entity.Property(e => e.Provinces).HasColumnType("TEXT");
        entity.Property(e => e.Seo).HasColumnType("TEXT").HasDefaultValue("{}");
        entity.Property(e => e.Stem).HasColumnType("VARCHAR");
        entity.Property(e => e.Subtitle).HasColumnType("VARCHAR");

        entity.Property(e => e.Hash).HasColumnName("__Hash__").HasColumnType("TEXT");
        entity.HasIndex(e => e.Hash).IsUnique();
    }
}