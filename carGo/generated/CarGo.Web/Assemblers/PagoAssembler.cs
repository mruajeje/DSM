// HARIA-WEB MVC managed assembler
using CarGo.Domain.Entities;

namespace CarGo.Web.Assemblers;

public static class PagoAssembler
{
    /*PROTECTED REGION ID(PagoAssembler_method_ToViewModel) START*/
    public static CarGo.Web.ViewModels.PagoViewModel ToViewModel(Pago entity) =>
        new()
        {
            Id = entity.Id,
            MontoMoneda = entity.Monto?.Moneda ?? default,
            MontoCantidad = entity.Monto?.Cantidad ?? default,
            FechaPago = entity.FechaPago,
            Estado = entity.Estado,
            IdTransaccionPasarela = entity.IdTransaccionPasarela,
            ReservaId = entity.Reserva?.Id,
            ReservaLabel = entity.Reserva is null ? null : Convert.ToString(entity.Reserva.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(PagoAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(CarGo.Web.ViewModels.PagoViewModel model, Pago entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.Monto ??= new global::CarGo.Domain.ValueObjects.Dinero();
        entity.Monto!.Moneda = model.MontoMoneda;
        entity.Monto!.Cantidad = model.MontoCantidad;
        entity.FechaPago = model.FechaPago;
        entity.Estado = model.Estado;
        entity.IdTransaccionPasarela = model.IdTransaccionPasarela;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(PagoAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
