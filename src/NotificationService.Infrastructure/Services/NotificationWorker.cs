using System.Text;
using System.Text.Json;
using Common.Base.DTO.Email;
using Common.Base.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.Infrastructure.Services;

public class NotificationWorker : BackgroundService, IAsyncDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<MessagingOptions> _options;
    private readonly ILogger<NotificationWorker> _logger;
    private readonly IConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    public NotificationWorker(ILogger<NotificationWorker> logger,
        IServiceProvider serviceProvider,
        IOptions<MessagingOptions> options)
    {
        _options = options;
        _serviceProvider = serviceProvider;
        _logger = logger;
       
        _connectionFactory = new ConnectionFactory()
        {
            HostName = _options.Value.HostName,
            UserName = _options.Value.UserName,
            Password = _options.Value.Password,
        };
        
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
    
    private async Task InitializeConnectionAsync()
    {
        _connection = await _connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        
        await _channel.QueueDeclareAsync(
            queue: _options.Value.QueueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null);

        _connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeConnectionAsync();
        if (_channel == null || _connection == null)
        {
            _logger.LogError("RabbitMQ (Consumer): Не удалось инициализировать соединение или канал.");
            return;
        }
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);

                var notification = JsonSerializer.Deserialize<SendEmailRequest>(messageJson);
                if (notification != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<INotificationHandler>();
                        await handler.HandleNotificationAsync(notification, stoppingToken);
                    }
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };
        await _channel.BasicConsumeAsync(_options.Value.QueueName, false, consumer, stoppingToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
        }
    }
}