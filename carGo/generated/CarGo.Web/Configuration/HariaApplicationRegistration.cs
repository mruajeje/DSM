// HARIA-WEB GENERATED sha256:a85101d0a07d2649268a3cd7508d72dce7ddf33b49c1033a0eadbc868c8c88cb
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CarGo.Web.Configuration;

public static class HariaApplicationRegistration
{
    public static IServiceCollection AddHariaApplication(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var connectionString = RequireConnectionString(configuration, environment);
        var nhibernate = new NHibernate.Cfg.Configuration();
        nhibernate.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
        nhibernate.SetProperty("dialect", "NHibernate.Dialect.SQLiteDialect");
        nhibernate.SetProperty("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
        nhibernate.SetProperty("adonet.batch_size", "100");
        nhibernate.SetProperty("show_sql", "true");
        nhibernate.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);
        nhibernate.AddAssembly(typeof(CarGo.Infrastructure.UnitOfWorkFactory).Assembly);
        services.AddSingleton(nhibernate.BuildSessionFactory());
        services.AddSingleton<CarGo.Application.Abstractions.IHariaPasswordHasher, CarGo.Infrastructure.Security.HariaPasswordHasher>();
        services.AddSingleton<CarGo.Application.Abstractions.IHariaTokenGenerator, CarGo.Infrastructure.Security.HariaTokenGenerator>();
        services.AddScoped<CarGo.Application.Abstractions.IUsuarioRepository, CarGo.Infrastructure.Repositories.UsuarioRepository>();
        services.AddSingleton<CarGo.Application.Abstractions.IUnitOfWorkFactory, CarGo.Infrastructure.UnitOfWorkFactory>();
        services.AddScoped<CarGo.Application.Services.VehiculoService>();
        services.AddScoped<CarGo.Application.Services.ViajeService>();
        services.AddScoped<CarGo.Application.Services.PagoService>();
        services.AddScoped<CarGo.Application.Services.MensajeService>();
        services.AddScoped<CarGo.Application.Services.ResenaService>();
        services.AddScoped<CarGo.Application.Services.NotificacionService>();
        services.AddScoped<CarGo.Application.Services.UsuarioService>();
        return services;
    }

    private static string RequireConnectionString(IConfiguration configuration, IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Configure ConnectionStrings:DbConnection in appsettings.Haria.json, user-secrets, or the ConnectionStrings__DbConnection environment variable.");
        }

        var sqlite = new System.Data.Common.DbConnectionStringBuilder { ConnectionString = connectionString };
        var dataSourceKey = sqlite.Keys.Cast<string>().FirstOrDefault(key =>
            string.Equals(key.Replace(" ", ""), "DataSource", StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, "Filename", StringComparison.OrdinalIgnoreCase));
        if (dataSourceKey is null)
        {
            throw new InvalidOperationException("The SQLite connection string must specify Data Source or Filename.");
        }
        var dataSource = Convert.ToString(sqlite[dataSourceKey]) ?? string.Empty;
        if (dataSource.Length == 0 || dataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase) || Path.IsPathRooted(dataSource)
            || string.Equals(dataSource, ":memory:", StringComparison.OrdinalIgnoreCase)
            || (sqlite.TryGetValue("Mode", out var mode) && string.Equals(Convert.ToString(mode), "Memory", StringComparison.OrdinalIgnoreCase)))
        {
            return sqlite.ConnectionString;
        }

        sqlite[dataSourceKey] = Path.GetFullPath(Path.Combine(environment.ContentRootPath, dataSource));
        return sqlite.ConnectionString;
    }
}
