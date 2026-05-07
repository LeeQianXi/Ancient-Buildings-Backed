using Buildings.Dtos;

namespace Buildings.Infrastructure.Data.Entities.Account;

public class UserAccountInfo
{
    public required long UserId { get; set; }
    public required string UserName { get; set; }
    public string Profile { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.Unknown;
    public ICollection<string> Interest { get; set; } = [];

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
}