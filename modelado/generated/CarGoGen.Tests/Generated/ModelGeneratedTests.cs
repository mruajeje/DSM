using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using CarGoGen.Application.Services;
using CarGoGen.Domain.Entities;
using CarGoGen.Infrastructure;
using CarGoGen.Infrastructure.Logging;
using Xunit.Abstractions;
using CarGoGen.Domain.ValueObjects;
using CarGoGen.Domain.Enums;

namespace CarGoGen.Tests.Generated;

public sealed class ModelGeneratedTests
{
    private readonly ITestOutputHelper _output;

    public ModelGeneratedTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void EnumMoneda_Should_Define_Model_Literals()
    {
        Assert.Equal(1, (int)EnumMoneda.EUR);
        Assert.Equal(2, (int)EnumMoneda.USD);
    }

    [Fact]
    public void EstadoViaje_Should_Define_Model_Literals()
    {
        Assert.Equal(1, (int)EstadoViaje.Publicado);
        Assert.Equal(2, (int)EstadoViaje.EnCurso);
        Assert.Equal(3, (int)EstadoViaje.Finalizado);
        Assert.Equal(4, (int)EstadoViaje.Cancelado);
    }

    [Fact]
    public void EstadoReserva_Should_Define_Model_Literals()
    {
        Assert.Equal(1, (int)EstadoReserva.Pendiente);
        Assert.Equal(2, (int)EstadoReserva.Aceptada);
        Assert.Equal(3, (int)EstadoReserva.Rechazada);
        Assert.Equal(4, (int)EstadoReserva.Cancelada);
    }

    [Fact]
    public void EstadoPago_Should_Define_Model_Literals()
    {
        Assert.Equal(1, (int)EstadoPago.Pendiente);
        Assert.Equal(2, (int)EstadoPago.Completado);
        Assert.Equal(3, (int)EstadoPago.Reembolsado);
    }

    [Fact]
    public void EstadoVerificacion_Should_Define_Model_Literals()
    {
        Assert.Equal(1, (int)EstadoVerificacion.Pendiente);
        Assert.Equal(2, (int)EstadoVerificacion.Verificado);
        Assert.Equal(3, (int)EstadoVerificacion.Rechazado);
    }

    [Fact]
    [Trait("Category", "Database")]
    public async Task Generated_Crud_And_Associations_Should_Work_Against_Database()
    {
        var cfg = CreateNHibernateConfiguration();
        new SchemaUpdate(cfg).Execute(false, true);
        using var loggerFactory = LoggerFactory.Create(logging => logging.SetMinimumLevel(LogLevel.Warning));
        using var unitOfWorkFactory = new UnitOfWorkFactory(cfg, loggerFactory);

            var usuarioService = new UsuarioService(unitOfWorkFactory);
            var sampleUsuario = await usuarioService.NewAsync("Sample Usuario NombreCompleto", "sample3@example.com", "Password123!", true, EstadoVerificacion.Pendiente);
            var sampleUsuarioLoginToken = await usuarioService.LoginAsync(sampleUsuario.Id, "Password123!");
            Ensure(!string.IsNullOrWhiteSpace(sampleUsuarioLoginToken), "Usuario.login returns a login token for sampleUsuario.");
            var sampleUsuarioReadBack = await usuarioService.ReadOIDAsync(sampleUsuario.Id);
            Ensure(sampleUsuarioReadBack is not null, "Usuario read by OID returns the generated entity.");
            var sampleUsuarioReadAll = await usuarioService.ReadAllAsync();
            Ensure(sampleUsuarioReadAll.Count > 0, "Usuario read all returns generated data.");
            await usuarioService.ModifyAsync(sampleUsuario.Id, "Sample Usuario NombreCompleto", "sample3@example.com", "Password123!", true, EstadoVerificacion.Pendiente);
            Ensure(await usuarioService.ReadOIDUsuarioDefaultAsync(sampleUsuario.Id) is not null, "Usuario modify keeps the entity readable.");

            var vehiculoService = new VehiculoService(unitOfWorkFactory);
            var sampleVehiculo = await vehiculoService.NewAsync("Sample Vehiculo MarcaModelo", "Sample Vehiculo Matricula", "Sample Vehiculo InformacionAdicional", sampleUsuario.Id);
            Ensure(sampleVehiculo.Propietario?.Equals(sampleUsuario) == true, "Vehiculo.new relates sampleVehiculo and sampleUsuario.");
            var sampleVehiculoReadBack = await vehiculoService.ReadOIDAsync(sampleVehiculo.Id);
            Ensure(sampleVehiculoReadBack is not null, "Vehiculo read by OID returns the generated entity.");
            var sampleVehiculoReadAll = await vehiculoService.ReadAllAsync();
            Ensure(sampleVehiculoReadAll.Count > 0, "Vehiculo read all returns generated data.");
            await vehiculoService.ModifyAsync(sampleVehiculo.Id, "Sample Vehiculo MarcaModelo", "Sample Vehiculo Matricula");
            Ensure(await vehiculoService.ReadOIDVehiculoDefaultAsync(sampleVehiculo.Id) is not null, "Vehiculo modify keeps the entity readable.");

            var viajeService = new ViajeService(unitOfWorkFactory);
            var sampleViaje = await viajeService.NewAsync(new Ubicacion { Ciudad = "Sample Ubicacion Ciudad", Direccion = "Sample Ubicacion Direccion", Latitud = 5.5, Longitud = 5.5 }, new Ubicacion { Ciudad = "Sample Ubicacion Ciudad", Direccion = "Sample Ubicacion Direccion", Latitud = 5.5, Longitud = 5.5 }, new DateTime(2026, 1, 5, 9, 0, 0), 5, new Dinero { Moneda = EnumMoneda.EUR, Cantidad = 5.5 }, true, true, EstadoViaje.Publicado, sampleUsuario.Id);
            Ensure(sampleViaje.Conductor?.Equals(sampleUsuario) == true, "Viaje.new relates sampleViaje and sampleUsuario.");
            var sampleViajeReadBack = await viajeService.ReadOIDAsync(sampleViaje.Id);
            Ensure(sampleViajeReadBack is not null, "Viaje read by OID returns the generated entity.");
            var sampleViajeReadAll = await viajeService.ReadAllAsync();
            Ensure(sampleViajeReadAll.Count > 0, "Viaje read all returns generated data.");
            await viajeService.ModifyAsync(sampleViaje.Id, new Ubicacion { Ciudad = "Sample Ubicacion Ciudad", Direccion = "Sample Ubicacion Direccion", Latitud = 5.5, Longitud = 5.5 }, new Ubicacion { Ciudad = "Sample Ubicacion Ciudad", Direccion = "Sample Ubicacion Direccion", Latitud = 5.5, Longitud = 5.5 }, new DateTime(2026, 1, 5, 9, 0, 0), 5, new Dinero { Moneda = EnumMoneda.EUR, Cantidad = 5.5 }, true, true, EstadoViaje.Publicado);
            Ensure(await viajeService.ReadOIDViajeDefaultAsync(sampleViaje.Id) is not null, "Viaje modify keeps the entity readable.");

            var mensajeService = new MensajeService(unitOfWorkFactory);
            var sampleMensaje = await mensajeService.NewAsync("Sample Mensaje Contenido", new DateTime(2026, 1, 8, 9, 0, 0), sampleViaje.Id, sampleUsuario.Id);
            Ensure(sampleMensaje.Viaje?.Equals(sampleViaje) == true, "Mensaje.new relates sampleMensaje and sampleViaje.");
            Ensure(sampleMensaje.Remitente?.Equals(sampleUsuario) == true, "Mensaje.new relates sampleMensaje and sampleUsuario.");
            var sampleMensajeReadBack = await mensajeService.ReadOIDAsync(sampleMensaje.Id);
            Ensure(sampleMensajeReadBack is not null, "Mensaje read by OID returns the generated entity.");
            var sampleMensajeReadAll = await mensajeService.ReadAllAsync();
            Ensure(sampleMensajeReadAll.Count > 0, "Mensaje read all returns generated data.");
            await mensajeService.ModifyAsync(sampleMensaje.Id, "Sample Mensaje Contenido", new DateTime(2026, 1, 8, 9, 0, 0));
            Ensure(await mensajeService.ReadOIDMensajeDefaultAsync(sampleMensaje.Id) is not null, "Mensaje modify keeps the entity readable.");

            var resenaService = new ResenaService(unitOfWorkFactory);
            var sampleResena = await resenaService.NewAsync(9, "Sample Resena Comentario", new DateTime(2026, 1, 9, 9, 0, 0), sampleViaje.Id, sampleUsuario.Id);
            Ensure(sampleResena.Viaje?.Equals(sampleViaje) == true, "Resena.new relates sampleResena and sampleViaje.");
            Ensure(sampleResena.Autor?.Equals(sampleUsuario) == true, "Resena.new relates sampleResena and sampleUsuario.");
            var sampleResenaReadBack = await resenaService.ReadOIDAsync(sampleResena.Id);
            Ensure(sampleResenaReadBack is not null, "Resena read by OID returns the generated entity.");
            var sampleResenaReadAll = await resenaService.ReadAllAsync();
            Ensure(sampleResenaReadAll.Count > 0, "Resena read all returns generated data.");
            await resenaService.ModifyAsync(sampleResena.Id, 9, new DateTime(2026, 1, 9, 9, 0, 0));
            Ensure(await resenaService.ReadOIDResenaDefaultAsync(sampleResena.Id) is not null, "Resena modify keeps the entity readable.");

            var notificacionService = new NotificacionService(unitOfWorkFactory);
            var sampleNotificacion = await notificacionService.NewAsync("Sample Notificacion Titulo", "Sample Notificacion Texto", new DateTime(2026, 1, 10, 9, 0, 0), sampleUsuario.Id);
            Ensure(sampleNotificacion.UsuarioNotificado?.Equals(sampleUsuario) == true, "Notificacion.new relates sampleNotificacion and sampleUsuario.");
            var sampleNotificacionReadBack = await notificacionService.ReadOIDAsync(sampleNotificacion.Id);
            Ensure(sampleNotificacionReadBack is not null, "Notificacion read by OID returns the generated entity.");
            var sampleNotificacionReadAll = await notificacionService.ReadAllAsync();
            Ensure(sampleNotificacionReadAll.Count > 0, "Notificacion read all returns generated data.");
            await notificacionService.ModifyAsync(sampleNotificacion.Id, "Sample Notificacion Titulo", "Sample Notificacion Texto", new DateTime(2026, 1, 10, 9, 0, 0));
            Ensure(await notificacionService.ReadOIDNotificacionDefaultAsync(sampleNotificacion.Id) is not null, "Notificacion modify keeps the entity readable.");

            // TODO: Set required relationship Pago.Reserva manually; it could not be ordered automatically.
            // TODO: No seed generated for Pago; one or more New arguments reference classes that were not seeded yet.

            await vehiculoService.DestroyAsync(sampleVehiculo.Id);
            await mensajeService.DestroyAsync(sampleMensaje.Id);
            await resenaService.DestroyAsync(sampleResena.Id);
            await viajeService.DestroyAsync(sampleViaje.Id);
            await notificacionService.DestroyAsync(sampleNotificacion.Id);
            await usuarioService.DestroyAsync(sampleUsuario.Id);

            Ensure((await usuarioService.ReadAllUsuarioDefaultAsync()).Count == 0, "Usuario seed cleanup completed.");
            Ensure((await vehiculoService.ReadAllVehiculoDefaultAsync()).Count == 0, "Vehiculo seed cleanup completed.");
            Ensure((await viajeService.ReadAllViajeDefaultAsync()).Count == 0, "Viaje seed cleanup completed.");
            Ensure((await mensajeService.ReadAllMensajeDefaultAsync()).Count == 0, "Mensaje seed cleanup completed.");
            Ensure((await resenaService.ReadAllResenaDefaultAsync()).Count == 0, "Resena seed cleanup completed.");
            Ensure((await notificacionService.ReadAllNotificacionDefaultAsync()).Count == 0, "Notificacion seed cleanup completed.");
    }

    private static Configuration CreateNHibernateConfiguration()
    {
        var configuration = new Configuration();
        var configPath = Path.Combine(AppContext.BaseDirectory, "hibernate.cfg.xml");
        if (File.Exists(configPath))
        {
            configuration.Configure(configPath);
        }
        else
        {
            configuration.Configure();
        }

        configuration.SetProperty(NHibernate.Cfg.Environment.ShowSql, "false");
        configuration.AddAssembly(typeof(UnitOfWorkFactory).Assembly);
        configuration.EnableSqlStatementCapture();
        return configuration;
    }

    private const string SuccessColor = "\u001b[32m";
    private const string ErrorColor = "\u001b[31m";
    private const string ResetColor = "\u001b[0m";

    private void Ensure(bool condition, string message)
    {
        WriteTestMessage(condition, message);
        Assert.True(condition, message);
    }

    private void WriteTestMessage(bool condition, string message)
    {
        var color = condition ? SuccessColor : ErrorColor;
        var status = condition ? "[OK] " : "[FAIL] ";
        _output.WriteLine($"{color}{status}{message}{ResetColor}");
    }

}
