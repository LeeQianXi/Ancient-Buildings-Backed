namespace Buildings.Responses.Building;

[Serializable]
public sealed record BuildingSummaryResponse
{
    public ArrayPair Provinces { get; set; } = new();
    public ArrayPair Categories { get; set; } = new();
    public ArrayPair Dynasties { get; set; } = new();
    public int Total { get; set; } = 0;
}

[Serializable]
public sealed record ArrayPair
{
    public int Length { get; set; }
    public ICollection<string> Values { get; set; } = [];
}

internal static class BuildingSummaryResponseExtensions
{
    extension(IEnumerable<string> values)
    {
        public ArrayPair WrapAsPair()
        {
            var value = values.ToArray();
            return new ArrayPair
            {
                Values = value,
                Length = value.Length
            };
        }
    }
}