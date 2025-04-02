namespace NotificationService.Domain.Constants;

public static class MessagingConstants
{
    public const string Section = "Rabbitmq";
    public const string HostName = $"{Section}:HostName";
    public const string UserName = $"{Section}:UserName";
    public const string Password = $"{Section}:Password";
    public const string QueueName = $"{Section}:QueueName";
}