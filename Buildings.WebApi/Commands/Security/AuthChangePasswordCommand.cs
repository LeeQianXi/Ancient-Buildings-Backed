using FluentValidation;

namespace Buildings.Commands.Security;

[Serializable]
public sealed record AuthChangePasswordCommand
{
    public required long UserId { get; init; }
    public required string NewPassword { get; init; }
    public required string ConfirmKey { get; init; }
}

internal sealed class AuthChangePasswordValidator : AbstractValidator<AuthChangePasswordCommand>
{
    public AuthChangePasswordValidator()
    {
        RuleFor(r => r.NewPassword)
            .NotEmpty().WithMessage("New password is required.");
        RuleFor(r => r.ConfirmKey)
            .NotEmpty().WithMessage("Confirm key is required.");
    }
}