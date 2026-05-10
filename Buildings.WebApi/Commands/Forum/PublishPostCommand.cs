namespace Buildings.Commands.Forum;

[Serializable]
public sealed record PublishPostCommand
{
    public required long AuthorId { get; init; }
    public required string Title { get; init; }
    public required string Tag { get; init; }
    public required string Data { get; init; }
}