using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGoGen.Application.Services_Viaje_Reserva_rechazarPasajero) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGoGen.Application.Services;

public sealed partial class ViajeService
{
    public async Task Reserva_rechazarPasajeroAsync(int p_oid, int p_conductor_oid)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.Services_Viaje_Reserva_rechazarPasajero) ENABLED START*/
        // 1. Obtener la reserva
        var reserva = await _unitOfWork.ReservaRepository.ReadAsync(p_oid);

        // 2. Validar que la reserva existe
        if (reserva == null)
        {
            throw new ModelException($"La reserva con ID {p_oid} no existe.");
        }

        // 3. Validar que el usuario que intenta rechazar es el conductor del viaje
        if (reserva.Viaje == null || reserva.Viaje.Conductor == null || reserva.Viaje.Conductor.Id != p_conductor_oid)
        {
            throw new ModelException("Solo el conductor del viaje puede rechazar una reserva.");
        }

        // 4. Validar que la reserva esté en estado Pendiente
        if (reserva.Estado != EstadoReserva.Pendiente)
        {
            throw new ModelException($"No se puede rechazar una reserva que se encuentra en estado {reserva.Estado}.");
        }

        // 5. Cambiar el estado a Rechazada
        reserva.Estado = EstadoReserva.Rechazada;

        // 6. Guardar cambios
        await _unitOfWork.ReservaRepository.SaveAsync(reserva);
        /*PROTECTED REGION END*/
    }
}