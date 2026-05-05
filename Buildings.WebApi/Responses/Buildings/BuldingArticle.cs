namespace Buildings.Responses.Buildings;

[Serializable]
public sealed record BuildingArticle
{
    public string Img { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ICollection<string> Provinces { get; set; } = [];
    public ICollection<string> Dynasties { get; set; } = [];
    public ICollection<string> Categories { get; set; } = [];
    public string Name { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}