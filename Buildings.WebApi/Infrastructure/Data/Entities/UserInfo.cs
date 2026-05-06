namespace Buildings.Infrastructure.Data.Entities;

public class UserInfo
{
    public required long UserId { get; set; }
    public required string UserName { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required bool Online { get; set; }
    public required string Location { get; set; } = string.Empty;
    public required string Avatar { get; set; } = string.Empty;
    public required ICollection<string> Tags { get; set; } = [];
}