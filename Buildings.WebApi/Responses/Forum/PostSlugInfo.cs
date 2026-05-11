using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Buildings.Responses.Account;
using Buildings.Utils;

namespace Buildings.Responses.Forum;

[Serializable]
public sealed record PostSlugInfo
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public required string Excerpt { get; init; }
    public bool IsAi { get; init; } = false;
    public required string Tag { get; init; }

    [IgnoreDataMember]
    [JsonIgnore]
    public DateTimeOffset CreatedAt
    {
        set => CreatedTime = value.ToRelativeTimeString();
    }

    public string CreatedTime { get; set; } = DateTimeOffset.UtcNow.ToRelativeTimeString();
    public required AccountPublicInfoResponse Author { get; init; }
    public required PostStats Stats { get; init; }
}

[Serializable]
public sealed record PostStats
{
    public int Views { get; set; } = 0;
    public int Likes { get; set; } = 0;
    public int CommentsCount { get; set; } = 0;
}