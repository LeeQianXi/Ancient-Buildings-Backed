using FluentValidation;

namespace Buildings.Commands.Security;

[Serializable]
public sealed record ResetPasswordCommand
{
    public required string Email { get; init; }
    public required string ConfirmKey { get; init; }
    public required string NewPassword { get; init; }
}

internal sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        RuleFor(r => r.ConfirmKey)
            .NotEmpty().WithMessage("Confirm key is required.");
        RuleFor(r => r.NewPassword)
            .NotEmpty().WithMessage("New password is required.");
    }
}