namespace Buildings.Commands.Account;

[Serializable]
public sealed record SplitPageFriendsCommand
{
    public int? Page { get; set; } = 1;
    public string? Search { get; set; } = string.Empty;
}