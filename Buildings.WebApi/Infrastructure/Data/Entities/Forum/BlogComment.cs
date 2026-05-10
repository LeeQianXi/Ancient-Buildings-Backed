using Buildings.Infrastructure.Data.Entities.Account;

namespace Buildings.Infrastructure.Data.Entities.Forum;

public class BlogComment
{
    public required long Id { get; init; }
    public required long PostId { get; init; }
    public required long AuthorId { get; init; }
    public required string Data { get; set; }
    public bool IsAi { get; init; } = false;
    public required DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? DeleteAt { get; set; }

    public virtual BlogPost BlogPost { get; set; } = null!;
    public virtual UserAccountInfo Author { get; set; } = null!;
}