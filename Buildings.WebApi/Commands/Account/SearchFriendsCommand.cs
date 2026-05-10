namespace Buildings.Commands.Account;

[Serializable]
public sealed record SearchFriendsCommand
{
    public int Page { get; init; } = 1;
    public ICollection<string> SearchTags { get; set; } = [];
    public ICollection<string> Searches { get; set; } = [];
}