using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace PhoneDirectory.ReportService.Messaging
{
    public class ReportPublisher
    {
        private readonly IConnection _conn;
        private readonly IModel _channel;
        private readonly string _queue;

        public ReportPublisher(IConfiguration cfg)
        {
            var host = cfg["RabbitMq:Host"] ?? "localhost";
            var port = int.Parse(cfg["RabbitMq:Port"] ?? "5672");
            var user = cfg["RabbitMq:User"] ?? "guest";
            var pass = cfg["RabbitMq:Pass"] ?? "guest";

            _queue = cfg["RabbitMq:Queue"];

            var factory = new ConnectionFactory { HostName = host, Port = port, UserName = user, Password = pass };
            _conn = factory.CreateConnection();
            _channel = _conn.CreateModel();
            _channel.QueueDeclare(queue: _queue, durable: true, exclusive: false, autoDelete: false);
        }

        public void Publish(ReportRequestedMessage msg)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg));
            var props = _channel.CreateBasicProperties();
            props.Persistent = true;

            _channel.BasicPublish(exchange: "", routingKey: _queue, basicProperties: props, body: body);
        }
    }
}
