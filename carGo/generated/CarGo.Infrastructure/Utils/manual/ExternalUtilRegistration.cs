using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CarGo.Application.Abstractions;
using CarGo.Infrastructure.Services;

namespace CarGo.Infrastructure.Utils.Manual;

public static class ExternalUtilRegistration
{
    public static void Register(
        IServiceCollection services,
        IConfiguration configuration)
    {
        _ = configuration;

        services.AddSingleton<IPasarelaPago, PasarelaPagoSimulada>();
    }
}