using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGoGen.Application.Services_Reserva_Reserva_aceptarPasajero) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGoGen.Application.Services;

public sealed partial class ViajeService
{
    public async Task Reserva_aceptarPasajeroAsync(int p_oid)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.Services_Viaje_Reserva_aceptarPasajero) ENABLED START*/

        // Buscar la reserva por su identificador

        //Comprobar que existe

        //Comprobar que el estado de la reserva es "Pendiente"

        //Obtener el viaje asociado

        //Comprobar que el viaje está en estado Publicado

        //Comprobar que el usuario que acepta es el conductor de ese viaje

        //Comprobar que quedan suficientes plazas

        //Restar las plazas reservadas a las disponibles del viaje

        //Guardar los cambios dentro de una misma transacción
        
        throw new ModelException("Application service generation for Reserva.Reserva_aceptarPasajero is pending.", new NotImplementedException("Method Reserva_aceptarPasajeroAsync() not yet implemented."));
        /*PROTECTED REGION END*/
    }
}
