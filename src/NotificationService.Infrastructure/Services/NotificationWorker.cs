using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using NotificationService.Domain.Constants;
using NotificationService.Domain.DTOs;
using NotificationService.Domain.Exceptions;
using NotificationService.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.Infrastructure.Services;

public class NotificationWorker : BackgroundService
{
    private readonly INotificationHandler _notificationHandler;
    private IConnection _connection;
    private IChannel _channel;
    private string? _queueName;

    public NotificationWorker(ISecretsProvider secretsProvider,
        INotificationHandler notificationHandler)
    {
        _notificationHandler = notificationHandler;
        
        var hostName = secretsProvider.GetSecret(
            MessagingConstants.HostName,
            "dev");
        var userName = secretsProvider.GetSecret(
            MessagingConstants.UserName,
            "dev");
        var password = secretsProvider.GetSecret(
            MessagingConstants.Password,
            "dev");
        _queueName = secretsProvider.GetSecret(
            MessagingConstants.QueueName,
            "dev");
        
        if (string.IsNullOrEmpty(hostName) ||
            string.IsNullOrEmpty(userName) ||
            string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(_queueName))
        {
            throw new VariableNotFoundException("Ошибка при создании клиента", 
                nameof(hostName), 
                nameof(NotificationWorker));
        }
        
        var factory = new ConnectionFactory()
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
        };
        
        _connection = factory.CreateConnectionAsync()
            .GetAwaiter()
            .GetResult();
        _channel = _connection.CreateChannelAsync()
            .GetAwaiter()
            .GetResult();

        _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null)
            .GetAwaiter()
            .GetResult();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);

                var notification = JsonSerializer.Deserialize<SendEmailRequest>(messageJson);
                if (notification != null)
                {
                    await _notificationHandler.HandleNotificationAsync(notification, stoppingToken);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };
        _channel.BasicConsumeAsync(_queueName, false, consumer, stoppingToken);
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public async void Dispose()
    {
        await _channel?.CloseAsync();
        await _connection?.CloseAsync();
    }
}