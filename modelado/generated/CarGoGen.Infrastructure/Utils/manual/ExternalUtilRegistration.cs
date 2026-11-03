using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CarGoGen.Application.Abstractions;
using CarGoGen.Infrastructure.Services;

namespace CarGoGen.Infrastructure.Utils.Manual;

public static class ExternalUtilRegistration
{
    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        _ = configuration;

        services.AddSingleton<IPasarelaPago, PasarelaPagoSimulada>();
    }
}