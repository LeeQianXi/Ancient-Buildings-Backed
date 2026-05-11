using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Buildings.Utils;

namespace Buildings.Responses.Forum;

[Serializable]
public sealed record CommentTreeNode
{
    public required long Id { get; set; }
    public required long AuthorId { get; set; }
    public required string AuthorName { get; set; }
    public required string Data { get; set; }
    public bool IsAi { get; init; } = false;

    [IgnoreDataMember]
    [JsonIgnore]
    public DateTimeOffset CreatedAt
    {
        set => CreatedTime = value.ToRelativeTimeString();
    }

    public string CreatedTime { get; set; } = DateTimeOffset.UtcNow.ToRelativeTimeString();

    public int ChildCount { get; set; } = 0;
    public ICollection<CommentTreeNode> Children { get; set; } = [];
}