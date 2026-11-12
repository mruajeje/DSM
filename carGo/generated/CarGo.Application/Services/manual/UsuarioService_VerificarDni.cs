using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGo.Application.Services_Usuario_verificarDni) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGo.Application.Services;

public sealed partial class UsuarioService
{
    public async Task VerificarDniAsync(int p_oid, string p_dni)
    {
        /*PROTECTED REGION ID(CarGo.Application.Services_Usuario_verificarDni) ENABLED START*/
        // Write here your custom code...

        throw new ModelException("Application service generation for Usuario.verificarDni is pending.", new NotImplementedException("Method VerificarDniAsync() not yet implemented."));
        /*PROTECTED REGION END*/
    }
}
