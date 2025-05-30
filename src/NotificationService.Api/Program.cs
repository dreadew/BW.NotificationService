using Common.Base.Constants;
using Common.Services.Configuration;
using NotificationService.Api;
using NotificationService.Application.Extension;
using NotificationService.Infrastructure.Extension;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddVaultConfiguration(options =>
{
    builder.Configuration.GetSection(VaultConstants.VaultSection).Bind(options);
});
builder.Services.AddHostedService<Worker>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var host = builder.Build();
host.Run();
