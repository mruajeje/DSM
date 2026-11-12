// HARIA-WEB MVC managed assembler
using CarGo.Domain.Entities;

namespace CarGo.Web.Assemblers;

public static class ViajeAssembler
{
    /*PROTECTED REGION ID(ViajeAssembler_method_ToViewModel) START*/
    public static CarGo.Web.ViewModels.ViajeViewModel ToViewModel(Viaje entity) =>
        new()
        {
            Id = entity.Id,
            OrigenCiudad = entity.Origen?.Ciudad ?? string.Empty,
            OrigenDireccion = entity.Origen?.Direccion ?? string.Empty,
            OrigenLatitud = entity.Origen?.Latitud ?? default,
            OrigenLongitud = entity.Origen?.Longitud ?? default,
            DestinoCiudad = entity.Destino?.Ciudad ?? string.Empty,
            DestinoDireccion = entity.Destino?.Direccion ?? string.Empty,
            DestinoLatitud = entity.Destino?.Latitud ?? default,
            DestinoLongitud = entity.Destino?.Longitud ?? default,
            FechaHoraSalida = entity.FechaHoraSalida,
            NumPlazasDisponibles = entity.NumPlazasDisponibles,
            PrecioMoneda = entity.Precio?.Moneda ?? default,
            PrecioCantidad = entity.Precio?.Cantidad ?? default,
            PermiteMascotas = entity.PermiteMascotas,
            PermiteEquipaje = entity.PermiteEquipaje,
            Observaciones = entity.Observaciones,
            Estado = entity.Estado,
            LatitudActual = entity.LatitudActual,
            LongitudActual = entity.LongitudActual,
            FechaUltimaPosicion = entity.FechaUltimaPosicion,
            ConductorId = entity.Conductor?.Id,
            ConductorLabel = entity.Conductor is null ? null : Convert.ToString(entity.Conductor.NombreCompleto, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ViajeAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(CarGo.Web.ViewModels.ViajeViewModel model, Viaje entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.Origen ??= new global::CarGo.Domain.ValueObjects.Ubicacion();
        entity.Origen!.Ciudad = model.OrigenCiudad;
        entity.Origen!.Direccion = model.OrigenDireccion;
        entity.Origen!.Latitud = model.OrigenLatitud;
        entity.Origen!.Longitud = model.OrigenLongitud;
        entity.Destino ??= new global::CarGo.Domain.ValueObjects.Ubicacion();
        entity.Destino!.Ciudad = model.DestinoCiudad;
        entity.Destino!.Direccion = model.DestinoDireccion;
        entity.Destino!.Latitud = model.DestinoLatitud;
        entity.Destino!.Longitud = model.DestinoLongitud;
        entity.FechaHoraSalida = model.FechaHoraSalida;
        entity.NumPlazasDisponibles = model.NumPlazasDisponibles;
        entity.Precio ??= new global::CarGo.Domain.ValueObjects.Dinero();
        entity.Precio!.Moneda = model.PrecioMoneda;
        entity.Precio!.Cantidad = model.PrecioCantidad;
        entity.PermiteMascotas = model.PermiteMascotas;
        entity.PermiteEquipaje = model.PermiteEquipaje;
        entity.Observaciones = model.Observaciones;
        entity.Estado = model.Estado;
        entity.LatitudActual = model.LatitudActual;
        entity.LongitudActual = model.LongitudActual;
        entity.FechaUltimaPosicion = model.FechaUltimaPosicion;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ViajeAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
