namespace Buildings.Dtos;

[Serializable]
public class AccountUserPublicInfos
{
    public required long UserId { get; init; }
    public required string UserName { get; init; }
    public required string Profile { get; set; } = string.Empty;
    public required string Location { get; set; } = string.Empty;
    public required Gender Gender { get; set; } = Gender.Unknown;
    public ICollection<string> Interest { get; set; } = [];
}