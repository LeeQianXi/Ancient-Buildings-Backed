using FluentValidation;

namespace Buildings.Commands.Security;

[Serializable]
public sealed record DeleteAccountCommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string ConfirmKey { get; init; }
}

internal sealed class DeleteAccountValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
        RuleFor(r => r.ConfirmKey)
            .NotEmpty().WithMessage("Confirm key is required.");
    }
}