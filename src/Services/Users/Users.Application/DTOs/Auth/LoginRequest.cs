using FastEndpoints;
using FluentValidation;

namespace Users.Application.DTOs.Auth;

public record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginRequestValidator : Validator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Поле електронної пошти не може бути порожнім")
            .EmailAddress().WithMessage("Недійсний формат електронної пошти");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не може бути порожнім")
            .MinimumLength(8).WithMessage("Пароль має містити щонайменше 8 символів");
    }
}