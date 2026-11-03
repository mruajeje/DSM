using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CarGoGen.Application.Abstractions;
using CarGoGen.Infrastructure.Utils.Manual;

namespace CarGoGen.Infrastructure.Utils.Generated;

public static class ExternalUtilsServiceCollectionExtensions
{
    public static IServiceCollection AddHariaExternalUtils(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWorkUtils, UnitOfWorkUtils>();
        ExternalUtilRegistration.Register(services, configuration);
        return services;
    }
}