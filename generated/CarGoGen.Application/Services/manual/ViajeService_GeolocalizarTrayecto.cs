using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGoGen.Application.Services_Viaje_geolocalizarTrayecto) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGoGen.Application.Services;

public sealed partial class ViajeService
{
    public async Task GeolocalizarTrayectoAsync(int p_oid, int p_usuario_oid, double p_latitud, double p_longitud, DateTime p_fecha)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.Services_Viaje_geolocalizarTrayecto) ENABLED START*/
       
        await _unitOfWork.BeginTransactionAsync("ViajeService.GeolocalizarTrayectoAsync");

        try
        {
            // Comprobar que el viaje existe
            Viaje viaje = await _repository.ReadAsync(p_oid)
                ?? throw new ModelException("Viaje not found.");

            // Para saber si el viaje esta activo
            if (viaje.Estado != EstadoViaje.EnCurso)
            {
                throw new ModelException("Solo se puede geolocalizar un viaje en curso.");
            }

            if (!(p_latitud >= -90 && p_latitud <= 90))
            {
                throw new ModelException("La latitud debe estar entre -90 y 90.");
            }

            if (!(p_longitud >= -180 && p_longitud <= 180))
            {
                throw new ModelException("La longitud debe estar entre -180 y 180.");
            }

            // Comprobar quien envia la posicion
            Usuario usuario = await _unitOfWork.UsuarioRepository.ReadAsync(p_usuario_oid)
                ?? throw new ModelException("Usuario not found.");

            bool esConductorDelViaje = viaje.Conductor is not null && viaje.Conductor.Id == usuario.Id;

            if (!usuario.EsAdmin && !esConductorDelViaje)
            {
                throw new ModelException("Solo el conductor del viaje o un administrador puede enviar la posición.");
            }

            // Actualizar la posicion del viaje
            viaje.LatitudActual = p_latitud;
            viaje.LongitudActual = p_longitud; 
            viaje.FechaUltimaPosicion = p_fecha;

            await _repository.SaveAsync(viaje);

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

        /*PROTECTED REGION END*/
    }
}
