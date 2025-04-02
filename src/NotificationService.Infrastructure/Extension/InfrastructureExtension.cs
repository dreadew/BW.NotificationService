using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Services;
using NotificationService.Domain.Constants;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Options;
using NotificationService.Infrastructure.Services;

namespace NotificationService.Infrastructure.Extension;

public static class InfrastructureExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<VaultOptions>(
            configuration.GetSection(VaultConstants.VaultSection));
        services.Configure<TotpOptions>(configuration.GetSection(TotpConstants.Section));
        services.AddSingleton<IVaultService, VaultService>();
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpConstants.Section));
        services.Configure<MessagingOptions>(configuration.GetSection
            (MessagingConstants.Section));
        //services.AddSingleton<ISecretsProvider, InfiscalSecretsProvider>();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddTransient<ITotpGenerator, TotpGenerator>();
        services.AddHostedService<NotificationWorker>();
    }
}