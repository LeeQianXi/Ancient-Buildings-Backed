namespace Buildings.Infrastructure.Data.Entities;

public class FriendRelation
{
    public required long FromUserId { get; set; }
    public required long TargetUserId { get; set; }
    public required DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? DeleteAt { get; set; }
}