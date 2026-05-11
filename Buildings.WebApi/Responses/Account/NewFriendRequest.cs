using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Buildings.Utils;

namespace Buildings.Responses.Account;

[Serializable]
public sealed record NewFriendRequest
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    [IgnoreDataMember]
    [JsonIgnore]
    public DateTimeOffset CreatedAt
    {
        set => CreatedTime = value.ToRelativeTimeString();
    }

    public string CreatedTime { get; set; } = DateTimeOffset.UtcNow.ToRelativeTimeString();
}