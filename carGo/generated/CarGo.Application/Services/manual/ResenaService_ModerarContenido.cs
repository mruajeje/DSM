using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGo.Application.Services_Resena_moderarContenido) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGo.Application.Services;

public sealed partial class ResenaService
{
    public async Task ModerarContenidoAsync(int p_oid, int p_admin_oid, EstadoResena p_decision, string p_motivo)
    {
        /*PROTECTED REGION ID(CarGo.Application.Services_Resena_moderarContenido) ENABLED START*/
        // Write here your custom code...

        throw new ModelException("Application service generation for Resena.moderarContenido is pending.", new NotImplementedException("Method ModerarContenidoAsync() not yet implemented."));
        /*PROTECTED REGION END*/
    }
}
