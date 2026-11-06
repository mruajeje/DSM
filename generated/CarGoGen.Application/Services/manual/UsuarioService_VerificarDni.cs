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
        
        // Comprobar que el DNI no este vacio
        if(string.IsNullOrWhiteSpace(p_dni))
        {
            throw new ModelException("El DNI es obligatorio.");
        }

        // Normalizar el DNI
        string dni = p_dni.Trim().ToUpperInvariant();

        // Comprobar el formato: ocho numero y una letra
        if(dni.Length != 9 || !dni[..8].All(char.IsAsciiDigit) || !char.IsAsciiLetter(dni[8]))
        {
            throw new ModelException("El DNI debe tener 8 números seguidos de una letra.");
        }

        // Comprobar que la letra corresponde al número
        const string letrasDni = "TRWAGMYFPDXBNJZSQVHLCKE";
        int numeroDni = int.Parse(dni[..8], System.Globalization.CultureInfo.InvariantCulture);

        char letraCorrecta = letrasDni[numeroDni % 23];

        if(dni[8] != letraCorrecta)
        {
            throw new ModelException("La letra del DNI no es correcta.");
        }

        // Buscar el usuario que quiere registrar su DNI
        Usuario entity = await _repository.ReadAsync(p_oid) ?? throw new ModelException("Usuario not found");

        // Impedir que el usuario cambie su DNI desde esta operación
        if(!string.IsNullOrWhiteSpace(entity.Dni))
        {
            if(entity.Dni.Equals(dni, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            throw new ModelException("El DNI de una cuenta no puede modificarse desde esta operación.");
        }

        // Comprobar que ningun otro usuario tenga ese DNI
        Usuario? usuarioConMismoDni = await _repository.ReadByDniAsync(dni);

        if(usuarioConMismoDni is not null && usuarioConMismoDni.Id != entity.Id)
        {
            throw new ModelException("El DNI ya está registrado en otra cuenta.");
        }

        // Guardar el DNI y dejarlo pediente de revision
        entity.Dni = dni;
        entity.EstadoVerificacion = EstadoVerificacion.Pendiente;

        await _repository.SaveAsync(entity);

        /*PROTECTED REGION END*/
    }
}
