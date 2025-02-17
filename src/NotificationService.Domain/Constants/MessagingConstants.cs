namespace NotificationService.Domain.Constants;

public static class MessagingConstants
{
    private const string RabbitmqSection = "Rabbitmq";
    public const string HostName = $"{RabbitmqSection}-HostName";
    public const string UserName = $"{RabbitmqSection}-UserName";
    public const string Password = $"{RabbitmqSection}-Password";
    public const string QueueName = $"{RabbitmqSection}-QueueName";
}