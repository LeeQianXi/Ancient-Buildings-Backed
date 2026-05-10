using Buildings.Dtos;

namespace Buildings.Responses.Forum;

[Serializable]
public sealed record PostDataResponse
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public bool IsAi { get; init; } = false;
    public required string Tag { get; init; }
    public required string Data { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public required AccountUserPublicInfos Author { get; init; }
    public required PostStats Stats { get; init; }
}