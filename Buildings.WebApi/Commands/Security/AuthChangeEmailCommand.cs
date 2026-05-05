using FluentValidation;

namespace Buildings.Commands.Security;

[Serializable]
public sealed record AuthChangeEmailCommand
{
    public required long UserId { get; init; }
    public required string NewEmail { get; init; }
}

internal sealed class AuthChangeEmailValidator : AbstractValidator<AuthChangeEmailCommand>
{
    public AuthChangeEmailValidator()
    {
        RuleFor(r => r.UserId)
            .NotEmpty().WithMessage("User ID is required.");
        RuleFor(r => r.NewEmail)
            .NotEmpty().WithMessage("NewEmail is required.")
            .EmailAddress().WithMessage("Invalid email address.");
    }
}