using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using PhoneDirectory.Core.Enums;
using PhoneDirectory.Core.Entities;
using PhoneDirectory.ReportService.Data;
using PhoneDirectory.ReportService.Messaging;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;

public class ReportRequestConsumer : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly IConfiguration _cfg;

    private RabbitMQ.Client.IConnection _conn;
    private RabbitMQ.Client.IModel _channel;
    private readonly string _queue;
    private readonly string ContactServiceBaseUrl;

    public ReportRequestConsumer(IServiceProvider sp, IConfiguration cfg)
    {
        _sp = sp;
        _cfg = cfg;

        var host = _cfg["RabbitMq:Host"] ?? "localhost";
        var port = int.TryParse(_cfg["RabbitMq:Port"], out var p) ? p : 5672;
        var user = _cfg["RabbitMq:User"] ?? "guest";
        var pass = _cfg["RabbitMq:Pass"] ?? "guest";
        _queue = _cfg["RabbitMq:Queue"] ?? "report-requests";
        ContactServiceBaseUrl = _cfg["ContactService:BaseUrl"];

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = user,
            Password = pass,
            DispatchConsumersAsync = true 
        };

        _conn = factory.CreateConnection();
        _channel = _conn.CreateModel();

        _channel.QueueDeclare(
            queue: _queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.BasicQos(prefetchSize: 0u, prefetchCount: 1, global: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var msg = JsonSerializer.Deserialize<ReportRequestedMessage>(json);
                if (msg is null)
                    throw new InvalidOperationException("ReportRequestedMessage deserialize failed.");

                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ReportDbContext>();

                var http = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>()
                                .CreateClient("contact");

                var resp = await http.GetAsync($"/api/person/stats?location={Uri.EscapeDataString(msg.Location)}", stoppingToken);

                resp.EnsureSuccessStatusCode();
                var stat = await resp.Content.ReadFromJsonAsync<LocationStatsDto>(cancellationToken: stoppingToken);
                if (stat is null)
                    throw new InvalidOperationException("LocationStatsDto deserialize failed.");

                var detail = new ReportDetail
                {
                    ReportRequestId = msg.ReportId,
                    Location = stat.Location,
                    PersonCount = stat.PersonCount,
                    PhoneNumberCount = stat.PhoneCount
                };
                await db.ReportDetails.AddAsync(detail, stoppingToken);

                var report = await db.ReportRequests
                                     .FirstOrDefaultAsync(r => r.Id == msg.ReportId, stoppingToken);

                if (report != null)
                {
                    report.Status = ReportStatus.Completed;
                    Console.WriteLine($"[Consumer] Report {report.Id} marked as Completed.");
                }
                else
                {
                    Console.WriteLine($"[Consumer] Report not found: {msg.ReportId}");
                }

                await db.SaveChangesAsync(stoppingToken);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Consumer] Error: {ex.GetType().Name} - {ex.Message}");
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(queue: _queue, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        try { _channel?.Close(); } catch {}
        try { _conn?.Close(); } catch { }
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