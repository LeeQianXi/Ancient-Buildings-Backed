namespace Buildings.Commands.Forum;

[Serializable]
public sealed record PublishPostCommand
{
    public required long AuthorId { get; init; }
    public required string Title { get; init; }
    public bool IsAi { get; init; } = false;
    public required string Data { get; init; }
}