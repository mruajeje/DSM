// HARIA-WEB MVC managed assembler
using CarGo.Domain.Entities;

namespace CarGo.Web.Assemblers;

public static class UsuarioAssembler
{
    /*PROTECTED REGION ID(UsuarioAssembler_method_ToViewModel) START*/
    public static CarGo.Web.ViewModels.UsuarioViewModel ToViewModel(Usuario entity) =>
        new()
        {
            Id = entity.Id,
            NombreCompleto = entity.NombreCompleto,
            Email = entity.Email,
            Telefono = entity.Telefono,
            Dni = entity.Dni,
            EsAdmin = entity.EsAdmin,
            EstadoVerificacion = entity.EstadoVerificacion,

        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(UsuarioAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(CarGo.Web.ViewModels.UsuarioViewModel model, Usuario entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.NombreCompleto = model.NombreCompleto;
        entity.Email = model.Email;
        entity.Telefono = model.Telefono;
        entity.Dni = model.Dni;
        entity.EsAdmin = model.EsAdmin;
        entity.EstadoVerificacion = model.EstadoVerificacion;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(UsuarioAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
