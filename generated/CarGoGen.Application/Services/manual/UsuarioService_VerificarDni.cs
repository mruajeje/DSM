using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGoGen.Application.Services_Usuario_verificarDni) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGoGen.Application.Services;

public sealed partial class UsuarioService
{
    public async Task VerificarDniAsync(int p_oid, string p_dni)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.Services_Usuario_verificarDni) ENABLED START*/
        // Write here your custom code...

        throw new ModelException("Application service generation for Usuario.verificarDni is pending.", new NotImplementedException("Method VerificarDniAsync() not yet implemented."));
        /*PROTECTED REGION END*/
    }
}
