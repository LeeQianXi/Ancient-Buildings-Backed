namespace Buildings.Responses.Buildings;

[Serializable]
public sealed record SplitPageResponse
{
    public ICollection<BuildingSlug> Items { get; set; } = [];
    public int Total { get; set; } = 0;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

[Serializable]
public sealed record BuildingSlug
{
    public string Hash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public ICollection<string> Provinces { get; set; } = [];
    public ICollection<string> Categories { get; set; } = [];
    public ICollection<string> Dynasties { get; set; } = [];
}