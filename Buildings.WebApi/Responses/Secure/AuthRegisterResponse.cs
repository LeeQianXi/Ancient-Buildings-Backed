using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Buildings.Utils;

namespace Buildings.Responses.Secure;

[Serializable]
public sealed record AuthRegisterResponse
{
    public required long UserId { get; init; }
    public required string Email { get; init; }
    public required string Username { get; init; }

    [IgnoreDataMember]
    [JsonIgnore]
    public DateTimeOffset CreatedAt
    {
        set => CreatedTime = value.ToRelativeTimeString();
    }

    public string CreatedTime { get; set; } = DateTimeOffset.UtcNow.ToRelativeTimeString();
}