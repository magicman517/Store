using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Users.Application.DTOs.Users;

public record CreateUserRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }

    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }

    public string? Phone { get; init; }
}

public class CreateUserRequestDtoValidator : Validator<CreateUserRequest>
{
    public CreateUserRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Поле електронної пошти не може бути порожнім")
            .EmailAddress().WithMessage("Недійсний формат електронної пошти");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не може бути порожнім")
            .MinimumLength(8).WithMessage("Пароль має містити щонайменше 8 символів")
            .Matches("[0-9]+").WithMessage("Пароль має містити щонайменше одну цифру");

        RuleFor(x => x.Phone)
            .Matches(@"^\+380\d{9}$")
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Телефонний номер має відповідати формату +(380)xxxxxxxxx");

        const string lettersOnlyRegex = @"^\p{L}+$";

        RuleFor(x => x.FirstName)
            .Matches(lettersOnlyRegex)
            .When(x => !string.IsNullOrEmpty(x.FirstName))
            .WithMessage("Ім'я має містити лише літери");

        RuleFor(x => x.LastName)
            .Matches(lettersOnlyRegex)
            .When(x => !string.IsNullOrEmpty(x.LastName))
            .WithMessage("Прізвище має містити лише літери");

        RuleFor(x => x.MiddleName)
            .Matches(lettersOnlyRegex)
            .When(x => !string.IsNullOrEmpty(x.MiddleName))
            .WithMessage("По батькові має містити лише літери");
    }
}