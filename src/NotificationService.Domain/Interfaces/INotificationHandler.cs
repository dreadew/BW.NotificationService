using Common.Base.DTO.Email;

namespace NotificationService.Domain.Interfaces;

public interface INotificationHandler
{
    Task HandleNotificationAsync(SendEmailRequest dto,
        CancellationToken cancellationToken = default);
}