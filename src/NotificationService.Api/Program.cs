using NotificationService.Api;
using NotificationService.Application.Extension;
using NotificationService.Infrastructure.Extension;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var host = builder.Build();
host.Run();
