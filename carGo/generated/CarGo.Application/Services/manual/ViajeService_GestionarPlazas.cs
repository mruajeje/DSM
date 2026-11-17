using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGo.Application.Services_Viaje_gestionarPlazas) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGo.Application.Services;

public sealed partial class ViajeService
{
    public async Task GestionarPlazasAsync(int p_oid, int p_nuevasPlazas)
    {
        /*PROTECTED REGION ID(CarGo.Application.Services_Viaje_gestionarPlazas) ENABLED START*/
        // Write here your custom code...

        // Comprobar que el numero de plazas no sea negativo
        if (p_nuevasPlazas < 0)
        {
            throw new ModelException("El numero de plazas no puede ser negativo.");
        }

        // Buscar el viaje
        Viaje entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Viaje not found.");

        // Contar los pasajeros actuales
        int numeroPasajeros = entity.Pasajero?.Count ?? 0;

        // No permitir menos plazas que pasajeros
        if (p_nuevasPlazas < numeroPasajeros)
        {
            throw new ModelException(
                "No se pueden reducir las plazas por debajo del numero de pasajeros actuales.");
        }

        // Actualizar las plazas
        entity.NumPlazasDisponibles = p_nuevasPlazas - numeroPasajeros;

        // Guardar los cambios
        await _repository.SaveAsync(entity);
        /*PROTECTED REGION END*/
    }
}
