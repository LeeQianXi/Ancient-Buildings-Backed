namespace Buildings.Responses.Secure;

public sealed record RefreshTokenResponse
{
    public required long UserId { get; init; }
    public required string AccessToken { get; init; }
    public required DateTimeOffset AccessTokenExpire { get; init; }
}