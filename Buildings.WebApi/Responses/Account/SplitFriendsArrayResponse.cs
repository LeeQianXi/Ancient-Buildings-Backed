namespace Buildings.Responses.Account;

[Serializable]
public sealed record SplitFriendsArrayResponse
{
    public ICollection<FriendInfo> Users { get; set; } = [];
    public int Count { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

[Serializable]
public sealed record FriendInfo
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Online { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public ICollection<string> Tags { get; set; } = [];
    public double MatchScore { get; set; }
}