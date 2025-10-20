using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Users.Application.DTOs.Users;

public class CreateUserRequest
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
    public CreateUserRequestDtoValidator(IStringLocalizer<CreateUserRequest> localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_ => localizer["Error.Email.NotEmpty"])
            .EmailAddress().WithMessage(_ => localizer["Error.Email.InvalidFormat"]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(_ => localizer["Error.Password.NotEmpty"])
            .MinimumLength(8).WithMessage(_ => localizer["Error.Password.MinLength"])
            .Matches("[0-9]+").WithMessage(_ => localizer["Error.Password.NoDigit"]);

        RuleFor(x => x.Phone)
            .Matches(@"^\+380\d{9}$")
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage(_ => localizer["Error.Phone.InvalidFormat"]);

        const string lettersOnlyRegex = @"^\p{L}+$";

        RuleFor(x => x.FirstName)
            .Matches(lettersOnlyRegex)
            .When(x => !string.IsNullOrEmpty(x.FirstName))
            .WithMessage(_ => localizer["Error.FirstName.LettersOnly"]);

        RuleFor(x => x.LastName)
            .Matches(lettersOnlyRegex)
            .When(x => !string.IsNullOrEmpty(x.LastName))
            .WithMessage(_ => localizer["Error.LastName.LettersOnly"]);

        RuleFor(x => x.MiddleName)
            .Matches(lettersOnlyRegex)
            .When(x => !string.IsNullOrEmpty(x.MiddleName))
            .WithMessage(_ => localizer["Error.MiddleName.LettersOnly"]);
    }
}