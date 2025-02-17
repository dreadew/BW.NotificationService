using NotificationService.Domain.DTOs;

namespace NotificationService.Domain.Interfaces;

public interface INotificationHandler
{
    Task HandleNotificationAsync(SendEmailRequest dto,
        CancellationToken cancellationToken = default);
}