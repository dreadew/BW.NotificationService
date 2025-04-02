namespace NotificationService.Domain.Options;

public class MessagingOptions
{
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string QueueName { get; set; }
}