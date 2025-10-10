using MediatR;

namespace Auth.Application.Users.Notifications;

public record UserCreatedNotification : INotification
{
    public Guid Id { get; init; }
}

public class UserCreatedNotificationHandler : INotificationHandler<UserCreatedNotification>
{
    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
    }
}