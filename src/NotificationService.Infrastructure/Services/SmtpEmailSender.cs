using MailKit.Net.Smtp;
using MimeKit;
using NotificationService.Domain.Constants;
using NotificationService.Domain.DTOs;
using NotificationService.Domain.Exceptions;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Infrastructure.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly string _from;
    private readonly string _host;
    private readonly string _port;
    private readonly string _user;
    private readonly string _password;

    public SmtpEmailSender(ISecretsProvider secretsProvider)
    {
        _from = secretsProvider.GetSecret(
            SmtpConstants.From,
            "dev");
        _host = secretsProvider.GetSecret(
            SmtpConstants.Host,
            "dev");
        _port = secretsProvider.GetSecret(
            SmtpConstants.Port,
            "dev");
        _user = secretsProvider.GetSecret(
            SmtpConstants.UserName,
            "dev");
        _password = secretsProvider.GetSecret(
            SmtpConstants.Password,
            "dev");

        if (string.IsNullOrEmpty(_from) ||
            string.IsNullOrEmpty(_host) ||
            string.IsNullOrEmpty(_port) ||
            string.IsNullOrEmpty(_user) ||
            string.IsNullOrEmpty(_password))
        {
            throw new VariableNotFoundException("Ошибка при создании SMTP клиента", 
                nameof(_from), nameof(SmtpEmailSender));
        }
}
    
    public async Task SendEmailAsync(EmailMessageDto dto,
        CancellationToken cancellationToken = default)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Notification Service", _from));
        emailMessage.To.Add(new MailboxAddress(dto.To, dto.To));
        emailMessage.Subject = dto.Subject;
        emailMessage.Body = new TextPart("html")
        {
            Text = dto.Body
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_host, int.Parse(_port), false, cancellationToken);
        await client.AuthenticateAsync(_user, _password, cancellationToken);
        await client.SendAsync(emailMessage, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}