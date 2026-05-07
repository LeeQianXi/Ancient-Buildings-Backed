namespace Buildings.Dtos;

public record Gender
{
    public static readonly Gender Unknown = new(0, "Unknown");
    public static readonly Gender Male = new(1, "Male");
    public static readonly Gender Female = new(2, "Female");
    public static readonly Gender Other = new(3, "Other");

    private Gender(int value, string code)
    {
        (Value, Code) = (value, code);
    }

    public int Value { get; }
    public string Code { get; }

    public static Gender FromValue(int value)
    {
        return value switch
        {
            0 => Unknown,
            1 => Male,
            2 => Female,
            3 => Other,
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
    }
}