using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Services;

namespace NotificationService.Infrastructure.Extension;

public static class InfrastructureExtension
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISecretsProvider, InfiscalSecretsProvider>();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddTransient<ITotpGenerator, TotpGenerator>();
        services.AddHostedService<NotificationWorker>();
    }
}