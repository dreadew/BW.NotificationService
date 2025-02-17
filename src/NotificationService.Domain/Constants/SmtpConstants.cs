namespace NotificationService.Domain.Constants;

public static class SmtpConstants
{
    private const string SmtpSection = "Smtp";
    public const string From = $"{SmtpSection}-Login";
    public const string Host = $"{SmtpSection}-Host";
    public const string Port = $"{SmtpSection}-Port";
    public const string UserName = $"{SmtpSection}-Email";
    public const string Password = $"{SmtpSection}-Password";
}