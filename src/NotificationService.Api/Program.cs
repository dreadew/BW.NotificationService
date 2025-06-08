using Common.Base.Constants;
using Common.Services.Configuration;
using Common.Services.ServiceExtensions;
using NotificationService.Api;
using NotificationService.Application.Extension;
using NotificationService.Infrastructure.Extension;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddVaultConfiguration(options =>
{
    builder.Configuration.GetSection(VaultConstants.VaultSection).Bind(options);
});
builder.Services.AddHostedService<Worker>();
builder.Services.AddTotpService(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var host = builder.Build();
host.Run();
