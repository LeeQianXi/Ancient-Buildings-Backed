namespace Buildings.Commands.Forum;

[Serializable]
public sealed record PublishCommentCommand
{
    public long RootId { get; set; } = 0;
    public long UserId { get; set; }
    public string Content { get; init; } = string.Empty;
}