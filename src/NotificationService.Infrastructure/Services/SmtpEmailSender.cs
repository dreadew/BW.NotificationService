using Common.Base.DTO.Email;
using Common.Base.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Infrastructure.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IOptions<SmtpOptions> _options;

    public SmtpEmailSender(IOptions<SmtpOptions> options)
    {
        _options = options;
    }

    public async Task SendEmailAsync(EmailMessageDto dto,
        CancellationToken cancellationToken = default)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_options.Value.From, _options.Value
            .UserName));
        emailMessage.To.Add(new MailboxAddress(dto.To, dto.To));
        emailMessage.Subject = dto.Subject;
        emailMessage.Body = new TextPart("html")
        {
            Text = dto.Body
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Value.Host, _options.Value.Port, true, 
            cancellationToken);
        await client.AuthenticateAsync(_options.Value.UserName, _options.Value.Password, 
            cancellationToken);
        await client.SendAsync(emailMessage, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}