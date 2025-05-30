using Common.Base.Constants;
using Common.Base.Options;
using Common.Services.ServiceExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Services;

namespace NotificationService.Infrastructure.Extension;

public static class InfrastructureExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddVault(configuration);
        services.AddTotpService(configuration);
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpConstants.Section));
        services.Configure<MessagingOptions>(configuration.GetSection
            (MessagingConstants.Section));
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddHostedService<NotificationWorker>();
    }
}