using System.Text;
using Common.Base.Constants;
using Common.Base.DTO.Email;
using Common.Base.Enums;
using Common.Base.Services;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services;

public class NotificationHandlerService : INotificationHandler
{
    private readonly ITotpService _totpService;
    private readonly IEmailSender _emailSender;

    public NotificationHandlerService(ITotpService totpService,
        IEmailSender emailSender)
    {
        _totpService = totpService;
        _emailSender = emailSender;
    }

    public async Task HandleNotificationAsync(SendEmailRequest dto,
        CancellationToken cancellationToken = default)
    {
        await SendConfirmationEmail(dto, cancellationToken);
    }

    private async Task SendConfirmationEmail(SendEmailRequest dto,
        CancellationToken cancellationToken = default)
    {
        if (!dto.Data.TryGetValue("Secret", out var secret))
        {
            throw new Exception("Invalid data");
        }

        var subject = dto.Type switch
        {
            NotificationTypes.ACCOUNT_CONFIRMATION => "Подтверждение аккаунта",
            NotificationTypes.PHONE_CONFIRMATION => "Подтверждение номера телефона",
            NotificationTypes.RECOVER_PASSWORD => "Восстановление пароля",
            NotificationTypes.RECOVER_LOGIN => "Восстановление логина",
            _ => "Подтверждение аккаунта"
        };

        var totp = _totpService.GenerateTotp(secret);
        var emailMessage = new EmailMessageDto(dto.Email,
            subject, FormatTotp(totp));
        await _emailSender.SendEmailAsync(emailMessage,
            cancellationToken);
    }

    private static string FormatTotp(string totp)
    {
        var sb = new StringBuilder(totp.Length + totp.Length % TotpConstants.ChunkSize);

        for (var i = 0; i < totp.Length; i += TotpConstants.ChunkSize)
        {
            sb.Append(totp.AsSpan(i, Math.Min(TotpConstants.ChunkSize, totp.Length - i)));
            if (i + TotpConstants.ChunkSize < totp.Length)
                sb.Append(TotpConstants.Separator);
        }
    
        return sb.ToString();
    }
}