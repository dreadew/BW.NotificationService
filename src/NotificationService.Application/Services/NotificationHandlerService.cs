using NotificationService.Domain.DTOs;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services;

public class NotificationHandlerService : INotificationHandler
{
    private readonly ITotpGenerator _totpGenerator;
    private readonly IEmailSender _emailSender;

    public NotificationHandlerService(ITotpGenerator totpGenerator,
        IEmailSender emailSender)
    {
        _totpGenerator = totpGenerator;
        _emailSender = emailSender;
    }

    public async Task HandleNotificationAsync(SendEmailRequest dto,
        CancellationToken cancellationToken = default)
    {
        switch (dto.Type)
        {
            case "AccountConfirmation":
                if (dto.Data.TryGetValue("Secret", out var secret))
                {
                    var totp = _totpGenerator.GenerateTotp(secret);
                    var emailMessage = new EmailMessageDto(dto.Email,
                        "Подтверждение аккаунта", totp);
                    await _emailSender.SendEmailAsync(emailMessage,
                        cancellationToken);
                }
                break;
            default:
                break;
        }
    }
}