using Microsoft.EntityFrameworkCore;
using PhoneDirectory.Core.Entities;
using PhoneDirectory.Core.Enums;
using PhoneDirectory.ReportService.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;


namespace PhoneDirectory.ReportService.Messaging
{
    public class ReportRequestConsumer : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly IConfiguration _cfg;
        private IConnection _conn;
        private IModel _channel;
        private string _queue;

        public ReportRequestConsumer(IServiceProvider sp, IConfiguration cfg)
        {
            _sp = sp; _cfg = cfg;
            var factory = new ConnectionFactory
            {
                HostName = cfg["RabbitMq:Host"] ?? "localhost",
                UserName = cfg["RabbitMq:User"] ?? "guest",
                Password = cfg["RabbitMq:Pass"] ?? "guest"
            };
            _queue = cfg["RabbitMq:Queue"] ?? "report-requests";
            _conn = factory.CreateConnection();
            _channel = _conn.CreateModel();
            _channel.QueueDeclare(queue: _queue, durable: true, exclusive: false, autoDelete: false);
            _channel.BasicQos(0, 1, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ReportDbContext>();
                var http = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("contact");

                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var msg = JsonSerializer.Deserialize<ReportRequestedMessage>(json)!;

                    var report = await db.ReportRequests.Include(r => r.ReportDetails)
                        .FirstOrDefaultAsync(r => r.Id == msg.ReportId);
                    if (report == null) { _channel.BasicAck(ea.DeliveryTag, false); return; }

                    var resp = await http.GetAsync($"/api/person/stats?location={Uri.EscapeDataString(msg.Location)}");
                    resp.EnsureSuccessStatusCode();
                    var stat = await resp.Content.ReadFromJsonAsync<LocationStatsDto>();

                    report.ReportDetails.Add(new ReportDetail
                    {
                        Location = stat!.Location,
                        PersonCount = stat.PersonCount,
                        PhoneNumberCount = stat.PhoneCount
                    });
                    report.Status = ReportStatus.Completed;
                    await db.SaveChangesAsync();

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch
                {
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: _queue, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _conn?.Dispose();
            base.Dispose();
        }
    }
    public class LocationStatsDto
    {
        public string Location { get; set; }
        public int PersonCount { get; set; }
        public int PhoneCount { get; set; }
    }
}
