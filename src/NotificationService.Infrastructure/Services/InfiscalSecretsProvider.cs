using Infisical.Sdk;
using Microsoft.Extensions.Configuration;
using NotificationService.Domain.Constants;
using NotificationService.Domain.Exceptions;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Infrastructure.Services;

public class InfiscalSecretsProvider : ISecretsProvider
{
        private readonly InfisicalClient _client;
        private readonly string? _projectId;
        
        public InfiscalSecretsProvider(IConfiguration configuration)
        {
                var infiscalSection = configuration.GetSection(SecretsProviderConstants.InfiscalSection);
                var clientId = infiscalSection[SecretsProviderConstants.ClientId];
                var clientSecret = infiscalSection[SecretsProviderConstants.ClientSecret];
                var siteUrl = infiscalSection[SecretsProviderConstants.SiteUrl];
                _projectId = infiscalSection[SecretsProviderConstants.ProjectId];

                if (string.IsNullOrEmpty(clientId) ||
                    string.IsNullOrEmpty(clientSecret) ||
                    string.IsNullOrEmpty(siteUrl) ||
                    string.IsNullOrEmpty(_projectId))
                {
                        throw new VariableNotFoundException("Ошибка при создании клиента", 
                                nameof(clientId), 
                                nameof(InfiscalSecretsProvider));
                }
                
                ClientSettings settings = new ClientSettings()
                {
                        Auth = new AuthenticationOptions()
                        {
                                UniversalAuth = new UniversalAuthMethod()
                                {
                                        ClientId = clientId,
                                        ClientSecret = clientSecret,
                                }
                        },
                        SiteUrl = siteUrl,
                };

                _client = new InfisicalClient(settings);
        }

        public string GetSecret(string key, string environment)
        {
                var getSecretOptions = new GetSecretOptions()
                {
                        SecretName = key,
                        ProjectId = _projectId,
                        Environment = environment
                };
                
                return _client.GetSecret(getSecretOptions).SecretValue;
        }
}