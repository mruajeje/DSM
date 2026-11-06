using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGoGen.Application.Services_Viaje_Reserva_cancelarReserva) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGoGen.Application.Services;

public sealed partial class ViajeService
{
    public async Task Reserva_cancelarReservaAsync(int p_oid, int p_usuario_oid)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.Services_Viaje_Reserva_cancelarReserva) ENABLED START*/
        // 1. Obtener la reserva por su ID
        var reserva = await _unitOfWork.ReservaRepository.ReadAsync(p_oid);

        // 2. Validar que la reserva existe
        if (reserva == null)
        {
            throw new ModelException($"La reserva con ID {p_oid} no existe.");
        }

        // 3. Comprobar que no esté ya cancelada o rechazada
        if (reserva.Estado == EstadoReserva.Cancelada || reserva.Estado == EstadoReserva.Rechazada)
        {
            throw new ModelException($"La reserva ya se encuentra en estado {reserva.Estado}.");
        }

        // 4. Validar permisos (debe ser el pasajero que la reservó o el conductor del viaje)
        bool esPasajero = reserva.Pasajero != null && reserva.Pasajero.Id == p_usuario_oid;
        bool esConductor = reserva.Viaje != null && reserva.Viaje.Conductor != null && reserva.Viaje.Conductor.Id == p_usuario_oid;

        if (!esPasajero && !esConductor)
        {
            throw new ModelException("No tienes permisos para cancelar esta reserva.");
        }

        // 5. Si la reserva estaba previamente Aceptada, liberar las plazas devueltas al Viaje
        if (reserva.Estado == EstadoReserva.Aceptada && reserva.Viaje != null)
        {
            reserva.Viaje.NumPlazasDisponibles += reserva.NumPlazasReservadas;
            await _unitOfWork.ViajeRepository.SaveAsync(reserva.Viaje);
        }

        // 6. Cambiar el estado a Cancelada
        reserva.Estado = EstadoReserva.Cancelada;

        // 7. Guardar cambios en el repositorio de reservas
        await _unitOfWork.ReservaRepository.SaveAsync(reserva);
        /*PROTECTED REGION END*/
    }
}