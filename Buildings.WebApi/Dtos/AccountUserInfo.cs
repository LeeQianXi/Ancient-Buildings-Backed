namespace Buildings.Dtos;

[Serializable]
public record AccountUserInfo
{
    public required long UserId { get; init; }
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}