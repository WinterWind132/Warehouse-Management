using Application;
using Application.MappingProfiles;
using Infrastructure;
using Infrastructure.MappingProfiles;
using Infrastructure.Presistence;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Warehouse.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Регистрируем модули
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

// Настройка Mapster
var config = new TypeAdapterConfig();
config.Scan(typeof(ApplicationMappingProfile).Assembly);
config.Scan(typeof(InfrastructureMappingProfile).Assembly);
builder.Services.AddSingleton(config);

var app = builder.Build();

// Добавляем middleware для обработки исключений
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Создаем базу данных если она не существует
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
    context.Database.EnsureCreated();
}

app.Run();