using FastEndpoints;
using FluentValidation;

namespace Gateway.Application.Dto.Users.Requests;

public record CreateUserRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }
    public string? Phone { get; init; }
}

public class CreateUserRequestValidator : Validator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Eлектронна пошта не може бути порожньою")
            .EmailAddress().WithMessage("Некоректний формат електронної пошти");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не може бути порожнім")
            .MinimumLength(8).WithMessage("Пароль повинен містити щонайменше 6 символів")
            .Matches("[0-9]+").WithMessage("Пароль повинен містити щонайменше одну цифру");

        RuleFor(x => x.Phone)
            .Matches(@"^\+380\d{9}$")
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Невірний формат телефону");
    }
}