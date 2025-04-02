namespace NotificationService.Domain.Constants;

public static class SmtpConstants
{
    public const string Section = "Smtp";
    public const string From = $"{Section}:Login";
    public const string Host = $"{Section}:Host";
    public const string Port = $"{Section}:Port";
    public const string UserName = $"{Section}:Email";
    public const string Password = $"{Section}:Password";
}