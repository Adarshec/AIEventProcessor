using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using API.DTOs;

namespace API.Messaging;

public class RabbitMqProducer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqProducer()
    {
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

    public void Send(CreateEventRequest request)
    {
        var message = JsonSerializer.Serialize(request);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: "",
            routingKey: "events_queue",
            basicProperties: null,
            body: body
        );
    }
}