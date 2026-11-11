using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGoGen.Application.Services_Notificacion_marcarComoLeida) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGoGen.Application.Services;

public sealed partial class NotificacionService
{
    public async Task MarcarComoLeidaAsync(int p_oid, int p_usuario_oid)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.Services_Notificacion_marcarComoLeida) ENABLED START*/
        // 1. Buscar la notificación por su ID con ReadAsync
        var notificacion = await _repository.ReadAsync(p_oid);

        // 2. Comprobar que existe
        if (notificacion == null)
        {
            throw new ModelException($"La notificación con ID {p_oid} no existe.");
        }

        // 3. Comprobar que pertenece al usuario indicado a través de la propiedad de navegación
        if (notificacion.UsuarioNotificado == null || notificacion.UsuarioNotificado.Id != p_usuario_oid)
        {
            throw new ModelException($"La notificación con ID {p_oid} no pertenece al usuario {p_usuario_oid}.");
        }

        // 4. Marcarla como leída
        notificacion.Leida = true;

        // 5. Guardar los cambios con SaveAsync
        await _repository.SaveAsync(notificacion);
        /*PROTECTED REGION END*/
    }
}