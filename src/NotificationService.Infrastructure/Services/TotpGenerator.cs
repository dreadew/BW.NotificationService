using NotificationService.Domain.DTOs;
using NotificationService.Domain.Interfaces;
using OtpNet;

namespace NotificationService.Infrastructure.Services;

public class TotpGenerator : ITotpGenerator
{
    public string GenerateTotp(string secret)
    {
        var secretBytes = Base32Encoding.ToBytes(secret);
        var totpCode = new Totp(secretBytes);
        return totpCode.ComputeTotp();
    }
}