namespace Buildings.Responses.Secure;

[Serializable]
public sealed record AuthRegisterResponse
{
    public required long UserId { get; init; }
    public required string Email { get; init; }
    public required string Username { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}