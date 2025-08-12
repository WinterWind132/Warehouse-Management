using Application.Interfaces;
using Infrastructure.Presistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContext<WarehouseDbContext>(options =>
                options.UseSqlServer(connectionString));
        }
        else
        {
            services.AddDbContext<WarehouseDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase")); // Для тестов только, мб удалю позже))
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IIncomeDocumentRepository, IncomeDocumentRepository>();
        services.AddScoped<IShipmentDocumentRepository, ShipmentDocumentsRepository>();
        services.AddScoped<IBalanceRepository, BalanceRepository>();
        
        return services;
    }
}