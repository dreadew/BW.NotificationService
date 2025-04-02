using Microsoft.Extensions.Options;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Options;
using OtpNet;

namespace NotificationService.Infrastructure.Services;

public class TotpGenerator : ITotpGenerator
{
    private readonly IOptions<TotpOptions>  _options;

    public TotpGenerator(IOptions<TotpOptions> options)
    {
        _options = options;
    }
    
    public string GenerateTotp(string secret)
    {
        Guid guidSecret = Guid.Parse(secret);
        var secretBytes = guidSecret.ToByteArray();
        
        var totpCode = new Totp(secretBytes,
            step: _options.Value.TimeStep,
            mode: OtpHashMode.Sha1,
            totpSize:  _options.Value.CodeLength);
        return totpCode.ComputeTotp();
    }
}