using System.Text.Json.Serialization;
using FastEndpoints;
using FluentValidation;
using Users.Core.Common;

namespace Users.Application.DTOs.Auth;

public record OauthRequest
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required OauthProvider Provider { get; init; }
    public required string ProviderUserId { get; init; }
    public required string Email { get; init; }
}

public class OauthRequestValidator : Validator<OauthRequest>
{
    public OauthRequestValidator()
    {
        RuleFor(x => x.Provider)
            .IsInEnum().WithMessage("Invalid OAuth provider");

        RuleFor(x => x.ProviderUserId)
            .NotEmpty().WithMessage("Provider user ID cannot be empty");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .EmailAddress().WithMessage("Invalid email format");
    }
}