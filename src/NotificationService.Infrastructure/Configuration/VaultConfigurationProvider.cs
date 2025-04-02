using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotificationService.Domain.Options;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace NotificationService.Infrastructure.Configuration;

public class VaultConfigurationProvider : ConfigurationProvider
{
    private readonly VaultOptions _options;

    public VaultConfigurationProvider(VaultOptions options)
    {
        _options = options;
    }

    public override void Load()
    {
        LoadAsync().GetAwaiter().GetResult();
    }

    private async Task LoadAsync()
    {
        var authMethod = new TokenAuthMethodInfo(_options.Token);
        var vaultClientSettings = new VaultClientSettings(_options.Address, authMethod);
        var vaultClient = new VaultClient(vaultClientSettings);

        try
        {
            var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync
                (_options.SecretPath, mountPoint: _options.MountPath);

            foreach (var item in secret.Data.Data)
            {
                var itemValue = item.Value.ToString();
                
                if (string.IsNullOrEmpty(itemValue))
                {
                    continue;
                }

                if (itemValue.StartsWith("{") || itemValue.StartsWith("["))
                {
                    try
                    {
                        var jToken = JToken.Parse(itemValue);
                        AddJsonToData(item.Key, jToken);
                        continue;
                    }
                    catch (JsonException)
                    {
                        
                    }
                }
                else if (item.Value is Dictionary<string, object> dictValue)
                {
                    var jObject = JObject.FromObject(dictValue);
                    AddJsonToData(item.Key, jObject);
                    continue;
                }
                
                Data[item.Key] = item.Value?.ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void AddJsonToData(string key, JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                foreach (var prop in token.Children<JProperty>())
                {
                    AddJsonToData($"{key}:{prop.Name}", prop.Value);
                }

                break;
            case JTokenType.Array:
                int index = 0;
                foreach (var value in token.Children())
                {
                    AddJsonToData($"{key}:{index}", value);
                    index++;
                }

                break;
            default:
                Data[key] = token.ToString();
                break;
        }
    }
}