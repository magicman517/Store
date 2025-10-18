using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Auth.Application.Dtos.Auth.Requests;

public record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginRequestValidator : Validator<LoginRequest>
{
    /// <summary>
    /// Initializes the validator with rules for LoginRequest and configures localized error messages for each rule.
    /// </summary>
    /// <param name="localizer">Provides localized strings for validation error messages.</param>
    public LoginRequestValidator(IStringLocalizer<LoginRequest> localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_ => localizer["Error.Email.NotEmpty"])
            .EmailAddress().WithMessage(_ => localizer["Error.Email.InvalidFormat"]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(_ => localizer["Error.Password.NotEmpty"]);
    }
}