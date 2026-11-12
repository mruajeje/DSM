// HARIA-WEB MVC managed assembler
using CarGo.Domain.Entities;

namespace CarGo.Web.Assemblers;

public static class ResenaAssembler
{
    /*PROTECTED REGION ID(ResenaAssembler_method_ToViewModel) START*/
    public static CarGo.Web.ViewModels.ResenaViewModel ToViewModel(Resena entity) =>
        new()
        {
            Id = entity.Id,
            Puntuacion = entity.Puntuacion,
            Comentario = entity.Comentario,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado,
            MotivoModeracion = entity.MotivoModeracion,
            ViajeId = entity.Viaje?.Id,
            ViajeLabel = entity.Viaje is null ? null : Convert.ToString(entity.Viaje.Observaciones, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            AutorId = entity.Autor?.Id,
            AutorLabel = entity.Autor is null ? null : Convert.ToString(entity.Autor.NombreCompleto, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ResenaAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(CarGo.Web.ViewModels.ResenaViewModel model, Resena entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.Puntuacion = model.Puntuacion;
        entity.Comentario = model.Comentario;
        entity.FechaCreacion = model.FechaCreacion;
        entity.Estado = model.Estado;
        entity.MotivoModeracion = model.MotivoModeracion;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ResenaAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
