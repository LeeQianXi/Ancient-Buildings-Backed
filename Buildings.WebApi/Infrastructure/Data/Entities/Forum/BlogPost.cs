using Buildings.Infrastructure.Data.Entities.Account;

namespace Buildings.Infrastructure.Data.Entities.Forum;

public class BlogPost
{
    public required long Id { get; init; }
    public required string Title { get; set; }
    public bool IsAi { get; init; } = false;
    public required string Tag { get; set; }
    public required string Data { get; set; }

    /// <summary>
    ///     用户创建时间
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    ///     用户删除时间,null表示未删除
    /// </summary>
    public DateTimeOffset? DeleteAt { get; set; }

    /// <summary>
    ///     用户信息更新时间
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    public int Views { get; set; } = 0;
    public int Likes { get; set; } = 0;

    public required long AuthorId { get; init; }
    public virtual UserAccountInfo Author { get; set; } = null!;
    public virtual ICollection<BlogComment> Comments { get; init; } = [];
}