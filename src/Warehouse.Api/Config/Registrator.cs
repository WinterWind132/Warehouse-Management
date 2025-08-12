using Application;
using Application.MappingProfiles;
using Infrastructure;
using Infrastructure.MappingProfiles;
using Mapster;

namespace Warehouse.Api.Config;

public static class Registrator
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApplicationServices()
            .AddInfrastructureServices(configuration);
        
        var config = new TypeAdapterConfig();
        config.Scan(typeof(ApplicationMappingProfile).Assembly);
        config.Scan(typeof(InfrastructureMappingProfile).Assembly);
        services.AddSingleton(config);
        return services;
    }
}
