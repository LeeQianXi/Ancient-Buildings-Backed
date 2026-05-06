namespace Buildings.Commands.Account;

[Serializable]
public sealed record SplitPageFriendsCommand
{
    public required int Page { get; set; } = 1;
    public required string Search { get; set; } = string.Empty;
}