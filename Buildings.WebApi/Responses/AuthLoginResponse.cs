namespace Buildings.Responses;

[Serializable]
public sealed record AuthLoginResponse
{
    public required long UserId { get; init; }
    public required string Email { get; init; }
    public required string AccessToken { get; init; }
    public required DateTimeOffset AccessTokenExpire { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTimeOffset RefreshTokenExpire { get; init; }
}