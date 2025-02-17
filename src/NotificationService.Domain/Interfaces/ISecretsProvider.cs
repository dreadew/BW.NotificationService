namespace NotificationService.Domain.Interfaces;

/// <summary>
/// Интерфейс для получения секретных ключей.
/// </summary>
public interface ISecretsProvider
{
    /// <summary>
    /// Синхронно возвращает значение секрета по ключу, идентификатору
    /// проекта и окружению.
    /// </summary>
    /// <param name="key">Ключ секретного значения</param>
    /// <param name="environment">Окружение</param>
    /// <returns>Секретное значение</returns>
    string GetSecret(string key, string environment);
}