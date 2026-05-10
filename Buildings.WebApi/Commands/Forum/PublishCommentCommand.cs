namespace Buildings.Commands.Forum;

[Serializable]
public sealed record PublishCommentCommand
{
    public long UserId { get; set; }
    public string Content { get; init; } = string.Empty;
}