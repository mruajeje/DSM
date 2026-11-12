using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGo.Application.Services_Reserva_Reserva_rechazarPasajero) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGo.Application.Services;

public sealed partial class ViajeService
{
    public async Task Reserva_rechazarPasajeroAsync(int p_oid, int p_conductor_oid)
    {
        /*PROTECTED REGION ID(CarGo.Application.Services_Viaje_Reserva_rechazarPasajero) ENABLED START*/
        // Write here your custom code...

        throw new ModelException("Application service generation for Reserva.Reserva_rechazarPasajero is pending.", new NotImplementedException("Method Reserva_rechazarPasajeroAsync() not yet implemented."));
        /*PROTECTED REGION END*/
    }
}
