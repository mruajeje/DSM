// HARIA-WEB MVC managed assembler
using CarGo.Domain.Entities;

namespace CarGo.Web.Assemblers;

public static class MensajeAssembler
{
    /*PROTECTED REGION ID(MensajeAssembler_method_ToViewModel) START*/
    public static CarGo.Web.ViewModels.MensajeViewModel ToViewModel(Mensaje entity) =>
        new()
        {
            Id = entity.Id,
            Contenido = entity.Contenido,
            FechaHora = entity.FechaHora,
            ViajeId = entity.Viaje?.Id,
            ViajeLabel = entity.Viaje is null ? null : Convert.ToString(entity.Viaje.Observaciones, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            RemitenteId = entity.Remitente?.Id,
            RemitenteLabel = entity.Remitente is null ? null : Convert.ToString(entity.Remitente.NombreCompleto, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(MensajeAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(CarGo.Web.ViewModels.MensajeViewModel model, Mensaje entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.Contenido = model.Contenido;
        entity.FechaHora = model.FechaHora;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(MensajeAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
