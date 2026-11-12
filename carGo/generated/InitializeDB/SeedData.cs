using CarGo.Application.Services;
using CarGo.Application.UseCases.Pago;
using CarGo.Domain.Entities;
using CarGo.Domain.ValueObjects;
using CarGo.Domain.Enums;
using CarGo.Infrastructure;
using CarGo.Infrastructure.Repositories;
using CarGo.Infrastructure.Security;
using Microsoft.Extensions.Logging;

/*PROTECTED REGION ID(seedDataUsings) ENABLED START*/
// Add custom usings here.
/*PROTECTED REGION END*/

namespace InitializeDB;

public static class SeedData
{
    public static async Task InitializeDataAsync(NHibernate.Cfg.Configuration cfg, ILoggerFactory? loggerFactory = null)
    {
        try
        {
            using var unitOfWorkFactory = new UnitOfWorkFactory(cfg, loggerFactory);

            var usuarioService = new UsuarioService(unitOfWorkFactory);
            var sampleUsuario = await usuarioService.NewAsync("Sample Usuario NombreCompleto", "sample3@example.com", "Password123!", true, EstadoVerificacion.Pendiente);
            var sampleUsuarioLoginToken = await usuarioService.LoginEmailAsync(sampleUsuario.Id, "Password123!");
            Ensure(!string.IsNullOrWhiteSpace(sampleUsuarioLoginToken), "Usuario.loginEmail returns a login token for sampleUsuario.");
            var sampleUsuarioReadBack = await usuarioService.ReadOIDAsync(sampleUsuario.Id);
            Ensure(sampleUsuarioReadBack is not null, "Usuario read by OID returns the generated entity.");
            var sampleUsuarioReadAll = await usuarioService.ReadAllAsync();
            Ensure(sampleUsuarioReadAll.Count > 0, "Usuario read all returns generated data.");
            var sampleUsuarioReadFilterByEmailResult = await usuarioService.ReadFilterByEmailAsync("sample3@example.com");
            Ensure(sampleUsuarioReadFilterByEmailResult is not null, "Usuario.readFilterByEmail filter executes.");
            var sampleUsuarioReadFilterByDniResult = await usuarioService.ReadFilterByDniAsync("Sample Usuario Dni");
            Ensure(sampleUsuarioReadFilterByDniResult is not null, "Usuario.readFilterByDni filter executes.");
            await usuarioService.ModifyAsync(sampleUsuario.Id, "Sample Usuario NombreCompleto", "sample3@example.com", "Password123!", true, EstadoVerificacion.Pendiente);
            Ensure(await usuarioService.ReadOIDAsync(sampleUsuario.Id) is not null, "Usuario modify keeps the entity readable.");

            var vehiculoService = new VehiculoService(unitOfWorkFactory);
            var sampleVehiculo = await vehiculoService.NewAsync("Sample Vehiculo MarcaModelo", "Sample Vehiculo Matricula", "Sample Vehiculo InformacionAdicional", sampleUsuario.Id);
            Ensure(sampleVehiculo.Propietario?.Equals(sampleUsuario) == true, "Vehiculo.new relates sampleVehiculo and sampleUsuario.");
            var sampleVehiculoReadBack = await vehiculoService.ReadOIDAsync(sampleVehiculo.Id);
            Ensure(sampleVehiculoReadBack is not null, "Vehiculo read by OID returns the generated entity.");
            var sampleVehiculoReadAll = await vehiculoService.ReadAllAsync();
            Ensure(sampleVehiculoReadAll.Count > 0, "Vehiculo read all returns generated data.");
            await vehiculoService.ModifyAsync(sampleVehiculo.Id, "Sample Vehiculo MarcaModelo", "Sample Vehiculo Matricula");
            Ensure(await vehiculoService.ReadOIDAsync(sampleVehiculo.Id) is not null, "Vehiculo modify keeps the entity readable.");

            var viajeService = new ViajeService(unitOfWorkFactory);
            var sampleViaje = await viajeService.NewAsync(new Ubicacion { Ciudad = "Sample Ubicacion Ciudad", Direccion = "Sample Ubicacion Direccion", Latitud = 5.5, Longitud = 5.5 }, new Ubicacion { Ciudad = "Sample Ubicacion Ciudad", Direccion = "Sample Ubicacion Direccion", Latitud = 5.5, Longitud = 5.5 }, new DateTime(2026, 1, 5, 9, 0, 0), 5, new Dinero { Moneda = EnumMoneda.EUR, Cantidad = 5.5 }, true, true, EstadoViaje.Publicado, sampleUsuario.Id);
            Ensure(sampleViaje.Conductor?.Equals(sampleUsuario) == true, "Viaje.new relates sampleViaje and sampleUsuario.");
            var sampleViajeReadBack = await viajeService.ReadOIDAsync(sampleViaje.Id);
            Ensure(sampleViajeReadBack is not null, "Viaje read by OID returns the generated entity.");
            var sampleViajeReadAll = await viajeService.ReadAllAsync();
            Ensure(sampleViajeReadAll.Count > 0, "Viaje read all returns generated data.");
            var sampleViajeReadFilterByOrigenDestinoResult = await viajeService.ReadFilterByOrigenDestinoAsync("Sample Viaje Origen", "Sample Viaje Destino");
            Ensure(sampleViajeReadFilterByOrigenDestinoResult is not null, "Viaje.readFilterByOrigenDestino filter executes.");
            var sampleViajeReadFilterByFechaYPlazasResult = await viajeService.ReadFilterByFechaYPlazasAsync(new DateTime(2026, 1, 5, 9, 0, 0), new DateTime(2026, 1, 5, 9, 0, 0), 5);
            Ensure(sampleViajeReadFilterByFechaYPlazasResult is not null, "Viaje.readFilterByFechaYPlazas filter executes.");
            var sampleViajeReadFilterByPrecioResult = await viajeService.ReadFilterByPrecioAsync(5.5, 5.5);
            Ensure(sampleViajeReadFilterByPrecioResult is not null, "Viaje.readFilterByPrecio filter executes.");
            var sampleViajeReadFilterByPreferenciasYEstadoResult = await viajeService.ReadFilterByPreferenciasYEstadoAsync(true, true);
            Ensure(sampleViajeReadFilterByPreferenciasYEstadoResult is not null, "Viaje.readFilterByPreferenciasYEstado filter executes.");
            var sampleViajeReadFilterViajesDisponiblesResult = await viajeService.ReadFilterViajesDisponiblesAsync(new DateTime(2026, 1, 5, 9, 0, 0));
            Ensure(sampleViajeReadFilterViajesDisponiblesResult is not null, "Viaje.readFilterViajesDisponibles filter executes.");
            var sampleViajeReadFilterByConductorVerificadoResult = await viajeService.ReadFilterByConductorVerificadoAsync();
            Ensure(sampleViajeReadFilterByConductorVerificadoResult is not null, "Viaje.readFilterByConductorVerificado filter executes.");
            await viajeService.ModifyAsync(sampleViaje.Id, new Ubicacion { Ciudad = "Sample Ubicacion Ciudad", Direccion = "Sample Ubicacion Direccion", Latitud = 5.5, Longitud = 5.5 }, new Ubicacion { Ciudad = "Sample Ubicacion Ciudad", Direccion = "Sample Ubicacion Direccion", Latitud = 5.5, Longitud = 5.5 }, new DateTime(2026, 1, 5, 9, 0, 0), 5, new Dinero { Moneda = EnumMoneda.EUR, Cantidad = 5.5 }, true, true, EstadoViaje.Publicado);
            Ensure(await viajeService.ReadOIDAsync(sampleViaje.Id) is not null, "Viaje modify keeps the entity readable.");

            var sampleReserva = await viajeService.Reserva_newAsync(6, EstadoReserva.Pendiente, new DateTime(2026, 1, 6, 9, 0, 0), sampleViaje.Id, sampleUsuario.Id);
            Ensure(sampleReserva.Viaje?.Equals(sampleViaje) == true, "Reserva.new relates sampleReserva and sampleViaje.");
            Ensure(sampleReserva.Pasajero?.Equals(sampleUsuario) == true, "Reserva.new relates sampleReserva and sampleUsuario.");
            var sampleReservaReadBack = await viajeService.Reserva_readOIDAsync(sampleReserva.Id);
            Ensure(sampleReservaReadBack is not null, "Reserva read by OID returns the generated entity.");
            var sampleReservaReadAll = await viajeService.Reserva_readAllAsync();
            Ensure(sampleReservaReadAll.Count > 0, "Reserva read all returns generated data.");
            var sampleReservaReadFilterByEstadoYFechaResult = await viajeService.Reserva_readFilterByEstadoYFechaAsync(EstadoReserva.Pendiente, new DateTime(2026, 1, 6, 9, 0, 0), new DateTime(2026, 1, 6, 9, 0, 0));
            Ensure(sampleReservaReadFilterByEstadoYFechaResult is not null, "Reserva.readFilterByEstadoYFecha filter executes.");
            await viajeService.Reserva_modifyAsync(sampleReserva.Id, 6, EstadoReserva.Pendiente, new DateTime(2026, 1, 6, 9, 0, 0));
            Ensure(await viajeService.Reserva_readOIDAsync(sampleReserva.Id) is not null, "Reserva modify keeps the entity readable.");

            var pagoService = new PagoService(unitOfWorkFactory);
            var samplePago = await pagoService.NewAsync(new Dinero { Moneda = EnumMoneda.EUR, Cantidad = 7.5 }, new DateTime(2026, 1, 7, 9, 0, 0), EstadoPago.Pendiente, sampleReserva.Id);
            Ensure(samplePago.Reserva?.Equals(sampleReserva) == true, "Pago.new relates samplePago and sampleReserva.");
            var samplePagoReadBack = await pagoService.ReadOIDAsync(samplePago.Id);
            Ensure(samplePagoReadBack is not null, "Pago read by OID returns the generated entity.");
            var samplePagoReadAll = await pagoService.ReadAllAsync();
            Ensure(samplePagoReadAll.Count > 0, "Pago read all returns generated data.");
            await pagoService.ModifyAsync(samplePago.Id, new Dinero { Moneda = EnumMoneda.EUR, Cantidad = 7.5 }, new DateTime(2026, 1, 7, 9, 0, 0), EstadoPago.Pendiente);
            Ensure(await pagoService.ReadOIDAsync(samplePago.Id) is not null, "Pago modify keeps the entity readable.");

            var mensajeService = new MensajeService(unitOfWorkFactory);
            var sampleMensaje = await mensajeService.NewAsync("Sample Mensaje Contenido", new DateTime(2026, 1, 8, 9, 0, 0), sampleViaje.Id, sampleUsuario.Id);
            Ensure(sampleMensaje.Viaje?.Equals(sampleViaje) == true, "Mensaje.new relates sampleMensaje and sampleViaje.");
            Ensure(sampleMensaje.Remitente?.Equals(sampleUsuario) == true, "Mensaje.new relates sampleMensaje and sampleUsuario.");
            var sampleMensajeReadBack = await mensajeService.ReadOIDAsync(sampleMensaje.Id);
            Ensure(sampleMensajeReadBack is not null, "Mensaje read by OID returns the generated entity.");
            var sampleMensajeReadAll = await mensajeService.ReadAllAsync();
            Ensure(sampleMensajeReadAll.Count > 0, "Mensaje read all returns generated data.");
            await mensajeService.ModifyAsync(sampleMensaje.Id, "Sample Mensaje Contenido", new DateTime(2026, 1, 8, 9, 0, 0));
            Ensure(await mensajeService.ReadOIDAsync(sampleMensaje.Id) is not null, "Mensaje modify keeps the entity readable.");

            var resenaService = new ResenaService(unitOfWorkFactory);
            var sampleResena = await resenaService.NewAsync(9, "Sample Resena Comentario", new DateTime(2026, 1, 9, 9, 0, 0), EstadoResena.Pendiente, sampleViaje.Id, sampleUsuario.Id);
            Ensure(sampleResena.Viaje?.Equals(sampleViaje) == true, "Resena.new relates sampleResena and sampleViaje.");
            Ensure(sampleResena.Autor?.Equals(sampleUsuario) == true, "Resena.new relates sampleResena and sampleUsuario.");
            var sampleResenaReadBack = await resenaService.ReadOIDAsync(sampleResena.Id);
            Ensure(sampleResenaReadBack is not null, "Resena read by OID returns the generated entity.");
            var sampleResenaReadAll = await resenaService.ReadAllAsync();
            Ensure(sampleResenaReadAll.Count > 0, "Resena read all returns generated data.");
            await resenaService.ModifyAsync(sampleResena.Id, 9, new DateTime(2026, 1, 9, 9, 0, 0), EstadoResena.Pendiente);
            Ensure(await resenaService.ReadOIDAsync(sampleResena.Id) is not null, "Resena modify keeps the entity readable.");

            var notificacionService = new NotificacionService(unitOfWorkFactory);
            var sampleNotificacion = await notificacionService.NewAsync("Sample Notificacion Titulo", "Sample Notificacion Texto", new DateTime(2026, 1, 10, 9, 0, 0), true, sampleUsuario.Id);
            Ensure(sampleNotificacion.UsuarioNotificado?.Equals(sampleUsuario) == true, "Notificacion.new relates sampleNotificacion and sampleUsuario.");
            var sampleNotificacionReadBack = await notificacionService.ReadOIDAsync(sampleNotificacion.Id);
            Ensure(sampleNotificacionReadBack is not null, "Notificacion read by OID returns the generated entity.");
            var sampleNotificacionReadAll = await notificacionService.ReadAllAsync();
            Ensure(sampleNotificacionReadAll.Count > 0, "Notificacion read all returns generated data.");
            await notificacionService.ModifyAsync(sampleNotificacion.Id, "Sample Notificacion Titulo", "Sample Notificacion Texto", new DateTime(2026, 1, 10, 9, 0, 0), true, sampleUsuario.Id);
            Ensure(await notificacionService.ReadOIDAsync(sampleNotificacion.Id) is not null, "Notificacion modify keeps the entity readable.");

            await viajeService.AddPasajeroAsync(sampleViaje.Id, new[] { sampleUsuario.Id });
            Ensure(sampleViaje.Pasajero.Contains(sampleUsuario), "Viaje.addPasajero relates Viaje and Usuario.");
            await viajeService.RemovePasajeroAsync(sampleViaje.Id, new[] { sampleUsuario.Id });
            Ensure(sampleViaje.Pasajero.Contains(sampleUsuario), "Viaje.removePasajero relates Viaje and Usuario.");

            await vehiculoService.DestroyAsync(sampleVehiculo.Id);
            await pagoService.DestroyAsync(samplePago.Id);
            await viajeService.Reserva_destroyAsync(sampleReserva.Id);
            await mensajeService.DestroyAsync(sampleMensaje.Id);
            await resenaService.DestroyAsync(sampleResena.Id);
            await viajeService.DestroyAsync(sampleViaje.Id);
            await notificacionService.DestroyAsync(sampleNotificacion.Id);
            await usuarioService.DestroyAsync(sampleUsuario.Id);

            Ensure((await usuarioService.ReadAllAsync()).Count == 0, "Usuario seed cleanup completed.");
            Ensure((await vehiculoService.ReadAllAsync()).Count == 0, "Vehiculo seed cleanup completed.");
            Ensure((await viajeService.ReadAllAsync()).Count == 0, "Viaje seed cleanup completed.");
            Ensure((await viajeService.Reserva_readAllAsync()).Count == 0, "Reserva seed cleanup completed.");
            Ensure((await pagoService.ReadAllAsync()).Count == 0, "Pago seed cleanup completed.");
            Ensure((await mensajeService.ReadAllAsync()).Count == 0, "Mensaje seed cleanup completed.");
            Ensure((await resenaService.ReadAllAsync()).Count == 0, "Resena seed cleanup completed.");
            Ensure((await notificacionService.ReadAllAsync()).Count == 0, "Notificacion seed cleanup completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException ?? ex);
            throw;
        }
    }

    public static async Task InitializeDataManualAsync(NHibernate.Cfg.Configuration cfg, ILoggerFactory? loggerFactory = null)
    {
        try
        {
            using var unitOfWorkFactory = new UnitOfWorkFactory(cfg, loggerFactory);

            /*PROTECTED REGION ID(initializeDataManual) ENABLED START*/
            // Datos de demo para el frontend (region protegida: sobrevive a regenerar).
            var usuarioSvc = new UsuarioService(unitOfWorkFactory);
            if ((await usuarioSvc.ReadAllAsync()).Count > 0) return; // idempotente: nunca duplica

            var vehiculoSvc = new VehiculoService(unitOfWorkFactory);
            var viajeSvc = new ViajeService(unitOfWorkFactory);
            var notifSvc = new NotificacionService(unitOfWorkFactory);

            var admin = await usuarioSvc.NewAsync("Admin CarGo", "admin@cargo.com", "Admin123!", true, EstadoVerificacion.Verificado);
            var ana = await usuarioSvc.NewAsync("Ana Garcia", "ana@cargo.com", "Ana12345!", false, EstadoVerificacion.Verificado);
            var luis = await usuarioSvc.NewAsync("Luis Martinez", "luis@cargo.com", "Luis12345!", false, EstadoVerificacion.Verificado);
            var marta = await usuarioSvc.NewAsync("Marta Lopez", "marta@cargo.com", "Marta12345!", false, EstadoVerificacion.Pendiente);

            await vehiculoSvc.NewAsync("Seat Leon", "1234-ABC", "Gris, 5 plazas", ana.Id);
            await vehiculoSvc.NewAsync("Renault Clio", "5678-DEF", "Azul, aire acondicionado", luis.Id);

            var alicante = new Ubicacion { Ciudad = "Alicante", Direccion = "Estacion de Autobuses", Latitud = 38.3452, Longitud = -0.4810 };
            var valencia = new Ubicacion { Ciudad = "Valencia", Direccion = "Estacio del Nord", Latitud = 39.4699, Longitud = -0.3763 };
            var madrid = new Ubicacion { Ciudad = "Madrid", Direccion = "Estacion de Atocha", Latitud = 40.4068, Longitud = -3.6892 };
            var barcelona = new Ubicacion { Ciudad = "Barcelona", Direccion = "Estacio de Sants", Latitud = 41.3791, Longitud = 2.1400 };
            var murcia = new Ubicacion { Ciudad = "Murcia", Direccion = "Estacion del Carmen", Latitud = 37.9876, Longitud = -1.1297 };

            var hoy = DateTime.Today;
            var v1 = await viajeSvc.NewAsync(alicante, valencia, hoy.AddDays(1).AddHours(9), 3, new Dinero { Moneda = EnumMoneda.EUR, Cantidad = 12.5 }, true, true, EstadoViaje.Publicado, ana.Id);
            var v2 = await viajeSvc.NewAsync(alicante, madrid, hoy.AddDays(2).AddHours(7), 4, new Dinero { Moneda = EnumMoneda.EUR, Cantidad = 25 }, false, true, EstadoViaje.Publicado, ana.Id);
            await viajeSvc.NewAsync(valencia, barcelona, hoy.AddDays(3).AddHours(8), 2, new Dinero { Moneda = EnumMoneda.EUR, Cantidad = 20 }, true, false, EstadoViaje.Publicado, luis.Id);
            await viajeSvc.NewAsync(murcia, alicante, hoy.AddDays(1).AddHours(18), 3, new Dinero { Moneda = EnumMoneda.EUR, Cantidad = 8 }, true, true, EstadoViaje.Publicado, luis.Id);
            await viajeSvc.NewAsync(madrid, valencia, hoy.AddDays(-5).AddHours(10), 0, new Dinero { Moneda = EnumMoneda.EUR, Cantidad = 18 }, false, true, EstadoViaje.Finalizado, luis.Id);
            await viajeSvc.NewAsync(barcelona, alicante, hoy.AddDays(4).AddHours(6), 3, new Dinero { Moneda = EnumMoneda.EUR, Cantidad = 30 }, true, true, EstadoViaje.Cancelado, ana.Id);

            await viajeSvc.Reserva_newAsync(1, EstadoReserva.Pendiente, DateTime.Now, v1.Id, marta.Id);
            await viajeSvc.Reserva_newAsync(2, EstadoReserva.Aceptada, DateTime.Now, v2.Id, marta.Id);

            await notifSvc.NewAsync("Bienvenida", "Bienvenido a CarGo", DateTime.Now, false, marta.Id);
            Console.WriteLine("Demo data inserted (4 usuarios, 2 vehiculos, 6 viajes, 2 reservas).");
            /*PROTECTED REGION END*/
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException ?? ex);
            throw;
        }
    }

    private static void Ensure(bool condition, string message)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = condition ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine((condition ? "[OK] " : "[FAIL] ") + message);
        Console.ForegroundColor = previousColor;

        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}