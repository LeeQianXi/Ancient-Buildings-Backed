namespace Buildings.Infrastructure.Data.Entities.Account;

public class FriendRelationInfo
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long FriendId { get; set; }
    public RequestStatus Status { get; set; }
    public long? ActionUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
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