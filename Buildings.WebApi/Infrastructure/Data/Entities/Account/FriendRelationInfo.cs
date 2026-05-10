namespace Buildings.Infrastructure.Data.Entities.Account;

public class FriendRelationInfo
{
    public required long Id { get; set; }
    public required long UserId { get; set; }
    public required long FriendId { get; set; }
    public RequestStatus Status { get; set; }
    public long? ActionUserId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual UserAccountInfo User { get; set; } = null!;
    public virtual UserAccountInfo Friend { get; set; } = null!;
}

public enum RequestStatus : short
{
    Pending,
    Accepted,
    Rejected,
    Deleted
}