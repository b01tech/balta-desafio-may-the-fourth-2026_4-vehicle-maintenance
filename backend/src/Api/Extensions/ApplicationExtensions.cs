using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ai.Agents;
using Ai.Configuration;
using Application.Services;
using Core.Interfaces;
using Core.Services;
using Infrastructure.Readers;

namespace Api.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCoreServices();
        services.AddInfrastructureServices();
        services.AddApplicationLayerServices();
        services.AddAiServices();
        
        return services;
    }

    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IMaintenanceCalculationService, MaintenanceCalculationService>();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IMileageDataReader, CsvMileageDataReader>();
        return services;
    }

    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddSingleton<IVehicleService, VehicleService>();
        services.AddScoped<IMaintenanceAnalysisUseCase, MaintenanceAnalysisUseCase>();
        return services;
    }

    public static IServiceCollection AddAiServices(this IServiceCollection services)
    {
        services.AddSingleton<IVehicleMaintenanceAgent, VehicleMaintenanceAgent>();
        return services;
    }
}