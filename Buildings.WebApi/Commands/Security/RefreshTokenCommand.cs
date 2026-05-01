using FluentValidation;

namespace Buildings.Commands.Security;

[Serializable]
public sealed record RefreshTokenCommand
{
    public required long UserId { get; init; }
    public required string Email { get; init; }
    public required string Hash { get; init; }
    public required string RefreshToken { get; init; }
}

internal sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(r => r.UserId)
            .NotEmpty().WithMessage("UserId is required.");
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        RuleFor(r => r.Hash)
            .NotEmpty().WithMessage("Hash is required.");
        RuleFor(r => r.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken is required.");
    }
}