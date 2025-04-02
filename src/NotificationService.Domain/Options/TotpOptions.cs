namespace NotificationService.Domain.Options;

public class TotpOptions
{
    public int TimeStep { get; set; }
    public int CodeLength { get; set; }
}