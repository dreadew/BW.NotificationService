using NotificationService.Domain.DTOs;

namespace NotificationService.Domain.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(EmailMessageDto dto,
        CancellationToken cancellationToken = default);
}