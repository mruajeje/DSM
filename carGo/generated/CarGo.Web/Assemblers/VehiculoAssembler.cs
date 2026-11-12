// HARIA-WEB MVC managed assembler
using CarGo.Domain.Entities;

namespace CarGo.Web.Assemblers;

public static class VehiculoAssembler
{
    /*PROTECTED REGION ID(VehiculoAssembler_method_ToViewModel) START*/
    public static CarGo.Web.ViewModels.VehiculoViewModel ToViewModel(Vehiculo entity) =>
        new()
        {
            Id = entity.Id,
            MarcaModelo = entity.MarcaModelo,
            Matricula = entity.Matricula,
            InformacionAdicional = entity.InformacionAdicional,
            PropietarioId = entity.Propietario?.Id,
            PropietarioLabel = entity.Propietario is null ? null : Convert.ToString(entity.Propietario.NombreCompleto, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(VehiculoAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(CarGo.Web.ViewModels.VehiculoViewModel model, Vehiculo entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.MarcaModelo = model.MarcaModelo;
        entity.Matricula = model.Matricula;
        entity.InformacionAdicional = model.InformacionAdicional;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(VehiculoAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
