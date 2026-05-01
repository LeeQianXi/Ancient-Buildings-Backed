using FluentValidation;

namespace Buildings.Commands.Security;

[Serializable]
public sealed record AuthRegisterCommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Username { get; init; }
}

internal sealed class AuthRegisterValidator : AbstractValidator<AuthRegisterCommand>
{
    public AuthRegisterValidator()
    {
        RuleFor(r => r.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(4).WithMessage("Username must be at least 4 characters long.");
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
    }
}