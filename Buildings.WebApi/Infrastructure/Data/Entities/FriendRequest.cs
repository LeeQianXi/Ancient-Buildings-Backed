namespace Buildings.Infrastructure.Data.Entities;

public class FriendRequest
{
    public required long FromUserId { get; set; }
    public required long TargetUserId { get; set; }
    public required string Description { get; set; }
    public bool? Status { get; set; }
}