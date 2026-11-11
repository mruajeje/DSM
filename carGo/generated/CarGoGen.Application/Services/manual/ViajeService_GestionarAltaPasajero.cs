using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGoGen.Application.Services_Viaje_gestionarAltaPasajero) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGoGen.Application.Services;

public sealed partial class ViajeService
{
    public async Task GestionarAltaPasajeroAsync(int p_oid, int p_usuario_oid)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.Services_Viaje_gestionarAltaPasajero) ENABLED START*/
        // Write here your custom code...

        // Buscar el viaje
            Viaje viaje = await _repository.ReadAsync(p_oid)
                ?? throw new ModelException("Viaje not found.");

            // Comprobar que quedan plazas libres
            if (viaje.NumPlazasDisponibles <= 0)
            {
                throw new ModelException("No quedan plazas disponibles en este viaje.");
            }

            // Buscar el usuario
            Usuario usuario = await _unitOfWork.UsuarioRepository.ReadAsync(p_usuario_oid)
                ?? throw new ModelException("Usuario not found.");

            // Comprobar que el usuario no está ya en el viaje
            if (viaje.Pasajero?.Contains(usuario) == true)
            {
                throw new ModelException("El usuario ya está añadido a este viaje.");
            }

            // Inicializar la lista de pasajeros
            viaje.Pasajero ??= new List<Usuario>();

            // Añadir el usuario al viaje
            viaje.Pasajero.Add(usuario);

            // Inicializar la lista de viajes del usuario
            usuario.Viajespasajero ??= new List<Viaje>();

            // Añadir el viaje a la lista del usuario
            if (!usuario.Viajespasajero.Contains(viaje))
            {
                usuario.Viajespasajero.Add(viaje);
            }

            // Restar una plaza libre
            viaje.NumPlazasDisponibles--;

            // Guardar los cambios
            await _unitOfWork.UsuarioRepository.SaveAsync(usuario);
            await _repository.SaveAsync(viaje);
        /*PROTECTED REGION END*/
    }
}
