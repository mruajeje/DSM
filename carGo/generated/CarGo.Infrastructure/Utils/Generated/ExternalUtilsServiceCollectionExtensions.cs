using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CarGo.Application.Abstractions;
using CarGo.Infrastructure.Utils.Manual;

namespace CarGo.Infrastructure.Utils.Generated;

public static class ExternalUtilsServiceCollectionExtensions
{
    public static IServiceCollection AddHariaExternalUtils(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWorkUtils, UnitOfWorkUtils>();
        ExternalUtilRegistration.Register(services, configuration);
        return services;
    }
}