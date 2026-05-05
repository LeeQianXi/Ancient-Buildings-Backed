namespace Buildings.Responses.Account;

[Serializable]
public sealed record AccountPublicInfo
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Online { get; set; }
    public string Location { get; set; } = string.Empty;
    public ICollection<string> Tags { get; set; } = [];
}