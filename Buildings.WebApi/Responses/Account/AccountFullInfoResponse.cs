namespace Buildings.Responses.Account;

[Serializable]
public sealed record AccountFullInfoResponse
{
    public required long UserId { get; init; }
    public required string UserName { get; init; }
    public required string Profile { get; set; } = string.Empty;
    public required string Location { get; set; } = string.Empty;
    public string Gender { get; set; } = Dtos.Gender.Unknown.Code;
    public ICollection<string> Interest { get; set; } = [];
    public required bool Online { get; set; }
}