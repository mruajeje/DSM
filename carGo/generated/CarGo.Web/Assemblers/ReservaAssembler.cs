// HARIA-WEB MVC managed assembler
using CarGo.Domain.Entities;

namespace CarGo.Web.Assemblers;

public static class ReservaAssembler
{
    /*PROTECTED REGION ID(ReservaAssembler_method_ToViewModel) START*/
    public static CarGo.Web.ViewModels.ReservaViewModel ToViewModel(Reserva entity) =>
        new()
        {
            Id = entity.Id,
            NumPlazasReservadas = entity.NumPlazasReservadas,
            Estado = entity.Estado,
            FechaSolicitud = entity.FechaSolicitud,
            ViajeId = entity.Viaje?.Id,
            ViajeLabel = entity.Viaje is null ? null : Convert.ToString(entity.Viaje.Observaciones, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            PasajeroId = entity.Pasajero?.Id,
            PasajeroLabel = entity.Pasajero is null ? null : Convert.ToString(entity.Pasajero.NombreCompleto, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            PagoId = entity.Pago?.Id,
            PagoLabel = entity.Pago is null ? null : Convert.ToString(entity.Pago.IdTransaccionPasarela, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ReservaAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(CarGo.Web.ViewModels.ReservaViewModel model, Reserva entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.NumPlazasReservadas = model.NumPlazasReservadas;
        entity.Estado = model.Estado;
        entity.FechaSolicitud = model.FechaSolicitud;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ReservaAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
