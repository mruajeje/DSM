using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGoGen.Application.Services_Viaje_gestionarBajaPasajero) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGoGen.Application.Services;

public sealed partial class ViajeService
{
    public async Task GestionarBajaPasajeroAsync(int p_oid, int p_usuario_oid)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.Services_Viaje_gestionarBajaPasajero) ENABLED START*/
        // Write here your custom code...

        // Buscar el viaje
        Viaje viaje = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Viaje not found.");

        // Buscar el usuario
        Usuario usuario = await _unitOfWork.UsuarioRepository.ReadAsync(p_usuario_oid)
            ?? throw new ModelException("Usuario not found.");

            // Comprobar que el usuario está añadido al viaje
        if (viaje.Pasajero?.Contains(usuario) != true)
        {
            throw new ModelException("El usuario no está añadido a este viaje.");
        }

        // Eliminar el usuario de la lista de pasajeros del viaje
        viaje.Pasajero.Remove(usuario);

        // Eliminar el viaje de la lista de viajes del usuario
        usuario.Viajespasajero?.Remove(viaje);

        // Devolver una plaza libre
        viaje.NumPlazasDisponibles++;

        // Guardar los cambios
        await _unitOfWork.UsuarioRepository.SaveAsync(usuario);
        await _repository.SaveAsync(viaje);
        /*PROTECTED REGION END*/
    }
}
