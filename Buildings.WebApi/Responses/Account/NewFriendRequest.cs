namespace Buildings.Responses.Account;

[Serializable]
public sealed record NewFriendRequest
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}