using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGo.Application.Services_Notificacion_marcarComoLeida) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGo.Application.Services;

public sealed partial class NotificacionService
{
    public async Task MarcarComoLeidaAsync(int p_oid, int p_usuario_oid)
    {
        /*PROTECTED REGION ID(CarGo.Application.Services_Notificacion_marcarComoLeida) ENABLED START*/
        // Write here your custom code...

        throw new ModelException("Application service generation for Notificacion.marcarComoLeida is pending.", new NotImplementedException("Method MarcarComoLeidaAsync() not yet implemented."));
        /*PROTECTED REGION END*/
    }
}
