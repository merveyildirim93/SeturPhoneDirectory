using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace PhoneDirectory.ReportService.Messaging
{
    public class RabbitPublisher : IRabbitPublisher
    {
        private readonly IConfiguration _cfg;
        private readonly string _queue;

        public RabbitPublisher(IConfiguration cfg)
        {
            _cfg = cfg;
            _queue = _cfg["RabbitMq:Queue"] ?? "report-requests";
        }

        public void Publish(object message)
        {
            var factory = new ConnectionFactory
            {
                HostName = _cfg["RabbitMq:Host"] ?? "localhost",
                Port = int.TryParse(_cfg["RabbitMq:Port"], out var p) ? p : 5672,
                UserName = _cfg["RabbitMq:User"] ?? "guest",
                Password = _cfg["RabbitMq:Pass"] ?? "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: _queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish(exchange: "", routingKey: _queue, basicProperties: null, body: body);
        }
    }
}
