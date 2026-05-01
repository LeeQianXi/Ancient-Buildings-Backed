using FluentValidation;

namespace Buildings.Commands.Security;

[Serializable]
public sealed record LoginAccountCommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

internal sealed class LoginAccountValidator : AbstractValidator<LoginAccountCommand>
{
    public LoginAccountValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}