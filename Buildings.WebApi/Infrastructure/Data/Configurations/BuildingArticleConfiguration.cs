using System.Text.Json;
using Buildings.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buildings.Infrastructure.Data.Configurations;

public class BuildingArticleConfiguration : IEntityTypeConfiguration<BuildingArticleData>
{
    public void Configure(EntityTypeBuilder<BuildingArticleData> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("BuildingArticleData");
        builder.HasKey(e => e.ArticleId)
            .HasName("PK_BuildingArticleData_ArticleId");

        builder.Property(e => e.ArticleId)
            .IsRequired()
            .ValueGeneratedNever();

        /* ---------- 字段 ---------- */
        builder.Property(e => e.Title)
            .IsRequired();
        builder.Property(e => e.SubTitle)
            .IsRequired();
        builder.Property(e => e.IsRed)
            .HasDefaultValue(false)
            .IsRequired();
        builder.Property(e => e.Seo)
            .IsRequired();
        builder.Property(e => e.Image)
            .IsRequired();
        builder.Property(e => e.Path)
            .IsRequired();
        builder.Property(e => e.Hash)
            .IsRequired();
        builder.Property(e => e.Data)
            .IsRequired();
        builder.Property(e => e.Description)
            .IsRequired();
        builder.Property(e => e.Provinces).HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );
        builder.Property(e => e.Categories).HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );
        builder.Property(e => e.Dynasties).HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );
        builder.Property(e => e.DisplayName)
            .IsRequired();
        /* ---------- 索引 ---------- */
        builder.HasIndex(e => e.Hash)
            .HasDatabaseName("IX_BuildingArticleData_Hash")
            .IsUnique();
        builder.HasIndex(e => e.IsRed)
            .HasDatabaseName("IX_BuildingArticleData_IsRed");
        builder.HasIndex(e => e.Path)
            .HasDatabaseName("IX_BuildingArticleData_Path")
            .IsUnique();
        builder.HasIndex(e => e.Provinces)
            .HasDatabaseName("IX_BuildingArticleData_Provinces")
            .HasMethod("GIN");
        builder.HasIndex(e => e.Categories)
            .HasDatabaseName("IX_BuildingArticleData_Categories")
            .HasMethod("GIN");
        builder.HasIndex(e => e.Dynasties)
            .HasDatabaseName("IX_BuildingArticleData_Dynasties")
            .HasMethod("GIN");
    }
}