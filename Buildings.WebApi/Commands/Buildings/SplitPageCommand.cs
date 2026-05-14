using FluentValidation;

namespace Buildings.Commands.Buildings;

[Serializable]
public sealed record SplitPageCommand
{
    public bool FilterRed { get; init; } = false;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public ICollection<string> Searches { get; set; } = [];
    public ICollection<string> Provinces { get; set; } = [];
    public ICollection<string> Categories { get; set; } = [];
    public ICollection<string> Dynasties { get; set; } = [];
}

internal class SplitPageValidator : AbstractValidator<SplitPageCommand>
{
    public SplitPageValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than zero");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than zero");
    }
}