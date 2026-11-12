using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGo.Application.Services_Viaje_geolocalizarTrayecto) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGo.Application.Services;

public sealed partial class ViajeService
{
    public async Task GeolocalizarTrayectoAsync(int p_oid, int p_usuario_oid, double p_latitud, double p_longitud, DateTime p_fecha)
    {
        /*PROTECTED REGION ID(CarGo.Application.Services_Viaje_geolocalizarTrayecto) ENABLED START*/
        // Write here your custom code...

        throw new ModelException("Application service generation for Viaje.geolocalizarTrayecto is pending.", new NotImplementedException("Method GeolocalizarTrayectoAsync() not yet implemented."));
        /*PROTECTED REGION END*/
    }
}
