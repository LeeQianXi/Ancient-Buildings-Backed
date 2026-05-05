using FluentValidation;

namespace Buildings.Commands.Security;

[Serializable]
public sealed record AuthResetPasswordCommand
{
    public required string Email { get; init; }
    public required string ConfirmKey { get; init; }
    public required string Password { get; init; }
    public required string NewPassword { get; init; }
}

internal sealed class AuthResetPasswordValidator : AbstractValidator<AuthResetPasswordCommand>
{
    public AuthResetPasswordValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        RuleFor(r => r.ConfirmKey)
            .NotEmpty().WithMessage("Confirm key is required.");
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("New password is required.");
        RuleFor(r => r.NewPassword)
            .NotEmpty().WithMessage("New password is required.");
    }
}