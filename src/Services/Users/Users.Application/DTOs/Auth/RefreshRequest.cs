using FastEndpoints;
using FluentValidation;

namespace Users.Application.DTOs.Auth;

public record RefreshRequest
{
    public required string RefreshToken { get; init; }
}

public class RefreshRequestValidator : Validator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Токен оновлення не може бути порожнім");
    }
}