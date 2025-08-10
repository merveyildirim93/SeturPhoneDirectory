using Microsoft.EntityFrameworkCore;
using PhoneDirectory.ReportService.Data;
using PhoneDirectory.ReportService.Messaging;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ReportDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ReportDbConnection")));


builder.Services.AddSingleton<ReportPublisher>();
builder.Services.AddHttpClient("contact", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ContactService:BaseUrl"]!);
});

builder.Services.AddScoped<IRabbitPublisher, RabbitPublisher>();


builder.Services.AddHostedService<ReportRequestConsumer>();


// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
