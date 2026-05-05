using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev.Entities;

[Table("_content_buildings")]
public class ContentBuilding
{
    [Key] public string Id { get; set; }

    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? Categories { get; set; }
    public string? Desc { get; set; }
    public string? Description { get; set; }
    public string? Dynasties { get; set; }
    public string? Extension { get; set; }
    public string? Img { get; set; }
    public string? Meta { get; set; }
    public string? Name { get; set; }

    // 这里用 string 类型，默认值通过 Fluent API 设置
    public string? Navigation { get; set; }

    public string? Path { get; set; }
    public string? Provinces { get; set; }

    // seo 默认值 '{}'，在数据库中已有默认值，EF Core 中可忽略设置
    public string? Seo { get; set; }

    public string? Stem { get; set; }
    public string? Subtitle { get; set; }

    // 或在 Fluent API 中配置 HasIndex().IsUnique()
    public string? Hash { get; set; }
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