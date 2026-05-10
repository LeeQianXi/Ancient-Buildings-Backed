namespace Buildings.Responses.Forum;

[Serializable]
public sealed record ForumSummaryResponse
{
    public ICollection<HotPostSlug> HotPostSlugs { get; init; } = [];
    public required int TotalPosts { get; init; }
    public required int TotalUsers { get; init; }
}

[Serializable]
public sealed record HotPostSlug
{
    public required long PostId { get; init; }
    public required string Title { get; init; }
    public bool IsAi { get; init; } = false;
    public required int HeatScore { get; init; }
}