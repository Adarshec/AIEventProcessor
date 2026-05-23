using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace API.Services;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMqConsumerService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: "events_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var request = JsonSerializer.Deserialize<CreateEventRequest>(message);

            if (request != null)
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var device = await context.Devices
                    .FirstOrDefaultAsync(x => x.DeviceIdText == request.DeviceId);

                if (device == null)
                {
                    device = new Device
                    {
                        Id = Guid.NewGuid(),
                        DeviceIdText = request.DeviceId
                    };

                    context.Devices.Add(device);
                    await context.SaveChangesAsync();
                }

                var newEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    DeviceId = device.Id,
                    Timestamp = request.Timestamp,
                    Temperature = request.Temperature,
                    Status = request.Status,
                    Metadata = JsonSerializer.Serialize(request.Metadata)
                };

                context.Events.Add(newEvent);
                await context.SaveChangesAsync();
            }

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(
            queue: "events_queue",
            autoAck: false,
            consumer: consumer
        );

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}