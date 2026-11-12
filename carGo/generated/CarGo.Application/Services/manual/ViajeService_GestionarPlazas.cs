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

        throw new ModelException("Application service generation for Viaje.gestionarPlazas is pending.", new NotImplementedException("Method GestionarPlazasAsync() not yet implemented."));
        /*PROTECTED REGION END*/
    }
}
