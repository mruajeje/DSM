using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

namespace CarGoGen.Application.Services;

public sealed partial class ViajeService
{
    public async Task Reserva_aceptarPasajeroAsync(int p_oid, int p_conductor_oid)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.Services_Viaje_Reserva_aceptarPasajero) ENABLED START*/

        // Todo el proceso se ejecuta en una misma transaccion
        await _unitOfWork.BeginTransactionAsync("ViajeService.Reserva_aceptarPasajeroAsync");        

        try
        {
            // Buscar la reserva por su identificador y comprobar que existe
            Reserva reserva = await _unitOfWork.ReservaRepository.ReadAsync(p_oid)
                ?? throw new ModelException("Reserva not found.");

            // Comprobar que el estado de la reserva es "Pendiente"
            if (reserva.Estado != EstadoReserva.Pendiente)
            {
                throw new ModelException("Solo se pueden aceptar reservas pendientes.");
            }

            // Obtener el viaje asociado
            Viaje viaje = reserva.Viaje
                ?? throw new ModelException("La reserva no tiene un viaje asociado.");

            // Comprobar que el viaje esta en estado Publicado
            if (viaje.Estado != EstadoViaje.Publicado)
            {
                throw new ModelException("Solo se pueden aceptar reservas de viajes publicados.");
            }

            // Comprobar que el usuario que acepta es el conductor de ese viaje
            // (solucion provisional: el conductor llega por parametro hasta que haya autenticacion)
            if (viaje.Conductor is null || viaje.Conductor.Id != p_conductor_oid)
            {
                throw new ModelException("Solo el conductor del viaje puede aceptar pasajeros.");
            }

            // Comprobar que la reserva pide al menos una plaza
            if (reserva.NumPlazasReservadas <= 0)
            {
                throw new ModelException("La reserva debe solicitar al menos una plaza.");
            }

            // Comprobar que quedan suficientes plazas
            if (viaje.NumPlazasDisponibles < reserva.NumPlazasReservadas)
            {
                throw new ModelException("No quedan plazas suficientes en el viaje.");
            }

            // Restar las plazas reservadas a las disponibles del viaje
            viaje.NumPlazasDisponibles -= reserva.NumPlazasReservadas;

            // Aceptar la reserva
            reserva.Estado = EstadoReserva.Aceptada;

            // Anyadir al pasajero al viaje (sin duplicarlo)
            Usuario pasajero = reserva.Pasajero
                ?? throw new ModelException("La reserva no tiene un pasajero asociado.");

            viaje.Pasajero ??= new List<Usuario>();
            if (!viaje.Pasajero.Any(u => u.Id == pasajero.Id))
            {
                viaje.Pasajero.Add(pasajero);
            }

            pasajero.Viajespasajero ??= new List<Viaje>();
            if (!pasajero.Viajespasajero.Any(v => v.Id == viaje.Id))
            {
                pasajero.Viajespasajero.Add(viaje);
            }

            // Crear la notificacion para el pasajero
            Notificacion notificacion = new Notificacion
            {
                Titulo = "Reserva aceptada",
                Texto = $"El conductor ha aceptado tu reserva de {reserva.NumPlazasReservadas} plaza(s) para el viaje de {viaje.Origen?.Ciudad} a {viaje.Destino?.Ciudad}.",
                FechaHora = DateTime.Now,
                UsuarioNotificado = pasajero,
            };

            pasajero.Notificaciones ??= new List<Notificacion>();
            pasajero.Notificaciones.Add(notificacion);

            // Guardar los cambios
            await _unitOfWork.ReservaRepository.SaveAsync(reserva);
            await _unitOfWork.ViajeRepository.SaveAsync(viaje);
            await _unitOfWork.UsuarioRepository.SaveAsync(pasajero);
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
