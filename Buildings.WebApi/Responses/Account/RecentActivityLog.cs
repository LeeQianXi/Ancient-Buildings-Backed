namespace Buildings.Responses.Account;

[Serializable]
public sealed record RecentActivityLog
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Decs { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
}