namespace Buildings.Responses.Account;

[Serializable]
public sealed record FriendsSummary
{
    public int Total { get; set; } = 0;
    public int Online { get; set; } = 0;
    public int Request { get; set; } = 0;
}