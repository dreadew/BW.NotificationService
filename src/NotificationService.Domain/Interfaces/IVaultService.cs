namespace NotificationService.Domain.Interfaces;

public interface IVaultService
{
    Task<T?> GetSecretAsync<T>(string key);
}