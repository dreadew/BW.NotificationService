using Microsoft.Extensions.Configuration;
using NotificationService.Domain.Options;

namespace NotificationService.Infrastructure.Configuration;

public static class VaultConfigurationExtensions
{
    public static IConfigurationBuilder AddVaultConfiguration(this
        IConfigurationBuilder configBuilder, Action<VaultOptions> setupAction)
    {
        var options = new VaultOptions();
        setupAction(options);

        configBuilder.Add(new VaultConfigurationSource(options));
        return configBuilder;
    }
}