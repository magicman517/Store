using Common.Exceptions;
using FluentValidation;
using MediatR;

namespace Auth.Application.Validation;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var firstFailure = validationResults
            .SelectMany(r => r.Errors)
            .FirstOrDefault(f => f is not null);

        if (firstFailure is null) return await next(cancellationToken);

        var message = string.IsNullOrWhiteSpace(firstFailure.ErrorMessage)
            ? "Validation error"
            : firstFailure.ErrorMessage;
        throw new ValidationApiException(message);
    }
}