using FluentValidation;

namespace Buildings.Commands.Security;

[Serializable]
public sealed record AuthLoginCommand
{
    public required string Email { get; init; }
    public required string Hash { get; init; }
    public required string Password { get; init; }
}

internal sealed class AuthLoginValidator : AbstractValidator<AuthLoginCommand>
{
    public AuthLoginValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        RuleFor(r => r.Hash)
            .NotEmpty().WithMessage("Hash is required.");
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}