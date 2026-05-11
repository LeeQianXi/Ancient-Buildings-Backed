using FluentValidation;

namespace Buildings.Commands.Forum;

[Serializable]
public sealed record SplitPageForumCommand
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
}

internal class SplitPageForumValidator : AbstractValidator<SplitPageForumCommand>
{
    public SplitPageForumValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than zero.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than zero.");
    }
}