using System.Text.Json;
using System.Text.Json.Nodes;

namespace Buildings.Infrastructure.Data.Entities;

public class BuildingArticleData
{
    public required long ArticleId { get; init; }
    public required string Title { get; set; }
    public required string SubTitle { get; set; }
    public required JsonDocument Seo { get; set; }
    public required string Image { get; set; }
    public required string Path { get; init; }
    public required string Hash { get; init; }
    public required JsonDocument Data { get; set; }
    public required string Description { get; set; }
    public ICollection<string> Provinces { get; set; } = [];
    public ICollection<string> Categories { get; set; } = [];
    public ICollection<string> Dynasties { get; set; } = [];
    public required string DisplayName { get; set; }
}