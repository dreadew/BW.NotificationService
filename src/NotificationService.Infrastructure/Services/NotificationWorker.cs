using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Domain.Constants;
using NotificationService.Domain.DTOs;
using NotificationService.Domain.Exceptions;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.Infrastructure.Services;

public class NotificationWorker : BackgroundService
{
    private readonly IOptions<MessagingOptions> _options;
    private readonly ILogger<NotificationWorker> _logger;
    private readonly INotificationHandler _notificationHandler;
    private IConnection _connection;
    private IChannel _channel;

    public NotificationWorker(ILogger<NotificationWorker> logger,
        INotificationHandler notificationHandler,
        IOptions<MessagingOptions> options)
    {
        _options = options;
        _notificationHandler = notificationHandler;
        _logger = logger;
       
        var factory = new ConnectionFactory()
        {
            HostName = _options.Value.HostName,
            UserName = _options.Value.UserName,
            Password = _options.Value.Password,
        };
        
        _connection = factory.CreateConnectionAsync()
            .GetAwaiter()
            .GetResult();
        _channel = _connection.CreateChannelAsync()
            .GetAwaiter()
            .GetResult();

        _channel.QueueDeclareAsync(
                queue: _options.Value.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null)
            .GetAwaiter()
            .GetResult();

        _connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;
        LogMessage(_options.Value.HostName, _options.Value.QueueName);
    }

    private void LogMessage(string hostName, string queueName)
    {
        _logger.LogInformation($"Подключение к RabbitMQ (consumer) установлено:\n - Хост: {hostName}\n - Очередь: {queueName}");
    }
    
    private Task OnConnectionShutdownAsync(object sender, 
        ShutdownEventArgs reason)
    {
        _logger.LogWarning("RabbitMQ (Consumer): соединение закрыто: {Reason}", 
            reason.ReplyText);
        return Task.CompletedTask;
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
        _channel.BasicConsumeAsync(_options.Value.QueueName, false, consumer, stoppingToken);
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public async void Dispose()
    {
        await _channel?.CloseAsync();
        await _connection?.CloseAsync();
    }
}