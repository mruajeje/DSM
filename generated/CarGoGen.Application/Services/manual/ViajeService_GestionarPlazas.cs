using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGoGen.Application.Services_Viaje_gestionarPlazas) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGoGen.Application.Services;

public sealed partial class ViajeService
{
    public async Task GestionarPlazasAsync(int p_oid, int p_nuevasPlazas)
    {
        Viaje entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Viaje not found.");

        if (p_nuevasPlazas < 0)
        {
            throw new ModelException("The number of available seats cannot be negative.");
        }
        entity.NumPlazasDisponibles = p_nuevasPlazas;
        await _repository.SaveAsync(entity);
    }
}
