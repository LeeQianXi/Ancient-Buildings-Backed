namespace Buildings.Responses.Building;

[Serializable]
public sealed record SplitPageResponse
{
    public ICollection<BuildingSlug> Items { get; set; } = [];
    public int Total { get; set; } = 0;
}

[Serializable]
public sealed record BuildingSlug
{
    public required string Hash { get; set; } = string.Empty;
    public required string Name { get; set; } = string.Empty;
    public required string Desc { get; set; } = string.Empty;
    public required bool IsRed { get; set; } = false;
    public required string Img { get; set; } = string.Empty;
    public required ICollection<string> Provinces { get; set; } = [];
    public required ICollection<string> Categories { get; set; } = [];
    public required ICollection<string> Dynasties { get; set; } = [];
}