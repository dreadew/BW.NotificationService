namespace NotificationService.Domain.Interfaces;

public interface ITotpGenerator
{
    /// <summary>
    /// Генерирует TOTP-код на основе секретного ключа.
    /// </summary>
    /// <param name="secret">Секретный ключ.</param>
    /// <returns>Сгенерированный TOTP-код.</returns>
    string GenerateTotp(string secret);
}