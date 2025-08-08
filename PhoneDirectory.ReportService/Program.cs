using Microsoft.EntityFrameworkCore;
using PhoneDirectory.ReportService.Data;
using PhoneDirectory.ReportService.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ReportDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ReportDbConnection")));


builder.Services.AddSingleton<ReportPublisher>();
builder.Services.AddHttpClient("contact", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ContactService:BaseUrl"]!);
});

builder.Services.AddHostedService<ReportRequestConsumer>();


// Add services to the container.

builder.Services.AddControllers();
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
