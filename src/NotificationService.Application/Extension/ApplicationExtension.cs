using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Extension;

public static class ApplicationExtension
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddTransient<INotificationHandler, NotificationHandlerService>();
    }
}