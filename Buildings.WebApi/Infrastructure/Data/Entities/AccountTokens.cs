namespace Buildings.Infrastructure.Data.Entities;

[Serializable]
public sealed record AccountTokens
{
    /// <summary>
    ///     用户唯一ID
    /// </summary>
    public required long UserId { get; init; }

    public required string Hash { get; init; }
    public required string? RefreshToken { get; set; }
    public required DateTimeOffset? RefreshTokenExpiry { get; set; }
}