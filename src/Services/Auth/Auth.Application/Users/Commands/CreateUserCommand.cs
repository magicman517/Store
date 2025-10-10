using Auth.Application.Common.Interfaces;
using Common.Exceptions;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace Auth.Application.Users.Commands;

public record CreateUserCommand : IRequest<Guid>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }
    public string? Phone { get; init; }
}

[UsedImplicitly]
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Електронна пошта обов'язкова")
            .EmailAddress().WithMessage("Невірний формат електронної пошти");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обов'язковий")
            .MinimumLength(8).WithMessage("Пароль повинен містити щонайменше 8 символів")
            .Matches("[0-9]").WithMessage("Пароль повинен містити щонайменше одну цифру");

        RuleFor(x => x.Phone)
            .Matches(@"^\+380\d{9}$")
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Невірний формат телефону");

        RuleFor(x => x.FirstName)
            .Matches(@"^\p{L}+$")
            .When(x => !string.IsNullOrEmpty(x.FirstName))
            .WithMessage("Ім'я може містити лише літери");

        RuleFor(x => x.LastName)
            .Matches(@"^\p{L}+$")
            .When(x => !string.IsNullOrEmpty(x.LastName))
            .WithMessage("Прізвище може містити лише літери");

        RuleFor(x => x.MiddleName)
            .Matches(@"^\p{L}+$")
            .When(x => !string.IsNullOrEmpty(x.MiddleName))
            .WithMessage("По батькові може містити лише літери"); // TODO
    }
}

public class CreateUserCommandHandler(IIdentityService identityService) : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await identityService.UserExistsAsync(request.Email, cancellationToken))
        {
            throw new ConflictException("Користувач з такою електронною поштою вже існує");
        }

        var isFirstUser = !await identityService.HasUsersAsync(cancellationToken);
        var role = isFirstUser ? "Admin" : "User";

        var userId = await identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.MiddleName,
            request.Phone,
            cancellationToken);

        await identityService.AddRoleAsync(role, cancellationToken);
        await identityService.AddToRoleAsync(userId, role, cancellationToken);

        return userId;
    }
}