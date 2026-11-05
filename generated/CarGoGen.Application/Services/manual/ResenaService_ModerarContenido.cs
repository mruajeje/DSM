using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGoGen.Application.Services_Resena_moderarContenido) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGoGen.Application.Services;

public sealed partial class ResenaService
{
    public async Task ModerarContenidoAsync(int p_oid, int p_admin_oid, EstadoResena p_decision, string p_motivo)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.Services_Resena_moderarContenido) ENABLED START*/
        
        await _unitOfWork.BeginTransactionAsync("ResenaService.ModerarContenidoAsync");

        try
        {
            // VALIDACIONES

            // Comprueba que la resenya existe
            Resena resena = await _repository.ReadAsync(p_oid) ?? throw new ModelException("Resena not found.");

            // Una vez moderada no se puede volver a moderar
            if(resena.Estado != EstadoResena.Pendiente)
            {
                throw new ModelException("Solo se pueden moderar reseñas pendientes.");
            }

            Usuario admin = await _unitOfWork.UsuarioRepository.ReadAsync(p_admin_oid) ?? throw new ModelException("Usuario not found.");

            if(!admin.EsAdmin)
            {
                throw new ModelException("Solo un administrador puede moderar reseñas.");
            }

            // Comprobar que el administrador no es el autor de la resenya
            if(resena.Autor.Id == p_admin_oid)
            {
                throw new ModelException("Un administrador no puede moderar su propia reseña.");
            }

            // Rechaza decisiones que nos ean las tres permitidas
            if(p_decision != EstadoResena.Aceptada && p_decision != EstadoResena.Oculta && p_decision != EstadoResena.Rechazada)
            {
                throw new ModelException("La decision debe ser Aceptada, Oculta o Rechazada.");
            }

            // Exige un motivo cuando se oculta o rechaza
            if(p_decision != EstadoResena.Aceptada && string.IsNullOrWhiteSpace(p_motivo))
            {
                throw new ModelException("Es obligatorio indicar el motivo si se oculta o rechaza la reseña.");
            }

            // ACTUALIZACION
            // Guarda el resultado de la moderacion
            resena.Estado = p_decision;
            // Por si el motivo viene vacio
            resena.MotivoModeracion = string.IsNullOrWhiteSpace(p_motivo) ? null : p_motivo.Trim();

            // Quita el contenido del comentario si la decision es ocultar -> en Aceptada y Rechazada el comentario se mantiene
            if(p_decision == EstadoResena.Oculta)
            {
                resena.Comentario = null;
            }

            await _repository.SaveAsync(resena);

            // Devuelve una valor distinto segun la decision
            string titulo = p_decision switch
            {
                EstadoResena.Aceptada => "Reseña aceptada",
                EstadoResena.Oculta => "Comentario de tu reseña ocultado",
                _ => "Reseña rechazada",
            
            };

            // Devuelve una valor distinto segun la decision
            string texto = p_decision switch
            {
                EstadoResena.Aceptada => "Tu reseña ha sido aceptada y ya es visible para otros usuarios.",
                EstadoResena.Oculta => $"Se ha ocultado el comentario de tu reseña. Motivo: {resena.MotivoModeracion}",
                _ => $"Tu reseña ha sido rechazada. Motivo: {resena.MotivoModeracion}",
            };

            // Se crea la notificacion con las cuatro propiedades que tiene la entidad
            Notificacion notificacion = new Notificacion
            {
                Titulo = titulo,
                Texto = texto,
                FechaHora = DateTime.UtcNow,
                UsuarioNotificado = resena.Autor,
            };

            await _unitOfWork.NotificacionRepository.SaveAsync(notificacion);
      
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
        
        /*PROTECTED REGION END*/
    }
}
