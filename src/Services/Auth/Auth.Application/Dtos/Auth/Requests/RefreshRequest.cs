using FastEndpoints;
using FluentValidation;

namespace Auth.Application.Dtos.Auth.Requests;

public record RefreshRequest
{
    public required string RefreshToken { get; init; }
}

public class RefreshRequestValidator : Validator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}