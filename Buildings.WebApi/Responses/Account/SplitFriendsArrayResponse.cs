namespace Buildings.Responses.Account;

[Serializable]
public sealed record SplitFriendsArrayResponse
{
    public ICollection<FriendInfo> Users { get; set; } = [];
    public int Count { get; set; }
}

[Serializable]
public sealed record FriendInfo
{
    public required long UserId { get; set; }
    public required string UserName { get; set; } = string.Empty;
    public string Profile { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public ICollection<string> Interest { get; set; } = [];
}