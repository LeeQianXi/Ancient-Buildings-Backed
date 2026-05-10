using Buildings.Dtos;

namespace Buildings.Responses.Forum;

public class ForumSplitPageResponse
{
    public required int TotalCount { get; init; }
    public ICollection<PostSlugInfo> DisplayedPosts { get; init; } = [];
}

[Serializable]
public sealed record PostSlugInfo
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public required string Excerpt { get; init; }
    public bool IsAi { get; init; } = false;
    public required string Tag { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public required AccountUserPublicInfos Author { get; init; }
    public required PostStats Stats { get; init; }
}

[Serializable]
public sealed record PostStats
{
    public int Views { get; set; } = 0;
    public int Likes { get; set; } = 0;
    public int CommentsCount { get; set; } = 0;
}