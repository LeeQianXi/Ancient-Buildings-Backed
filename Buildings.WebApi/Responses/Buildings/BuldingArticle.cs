using System.Text.Json;

namespace Buildings.Responses.Buildings;

[Serializable]
public sealed record BuildingArticle
{
    public required string Img { get; set; } = string.Empty;
    public required string Title { get; set; } = string.Empty;
    public required ICollection<string> Provinces { get; set; } = [];
    public required ICollection<string> Dynasties { get; set; } = [];
    public required ICollection<string> Categories { get; set; } = [];
    public required string Name { get; set; } = string.Empty;
    public required string Desc { get; set; } = string.Empty;
    public required string Subtitle { get; set; } = string.Empty;
    public required string Id { get; set; } = string.Empty;
    public required JsonDocument Body { get; set; }
}