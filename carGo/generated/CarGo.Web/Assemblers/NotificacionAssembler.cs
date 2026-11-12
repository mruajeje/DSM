// HARIA-WEB MVC managed assembler
using CarGo.Domain.Entities;

namespace CarGo.Web.Assemblers;

public static class NotificacionAssembler
{
    /*PROTECTED REGION ID(NotificacionAssembler_method_ToViewModel) START*/
    public static CarGo.Web.ViewModels.NotificacionViewModel ToViewModel(Notificacion entity) =>
        new()
        {
            Id = entity.Id,
            Titulo = entity.Titulo,
            Texto = entity.Texto,
            FechaHora = entity.FechaHora,
            Leida = entity.Leida,
            UsuarioNotificadoId = entity.UsuarioNotificado?.Id,
            UsuarioNotificadoLabel = entity.UsuarioNotificado is null ? null : Convert.ToString(entity.UsuarioNotificado.NombreCompleto, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(NotificacionAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(CarGo.Web.ViewModels.NotificacionViewModel model, Notificacion entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.Titulo = model.Titulo;
        entity.Texto = model.Texto;
        entity.FechaHora = model.FechaHora;
        entity.Leida = model.Leida;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(NotificacionAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
