namespace Buildings.Commands.Friends;

[Serializable]
public sealed record SearchFriendsCommand
{
    public ICollection<string> SearchTags { get; set; } = [];
    public ICollection<string> Searches { get; set; } = [];
}