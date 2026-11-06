using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Application.Services;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGoGen.Application.UseCases_Pago_realizarPago) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGoGen.Application.UseCases.Pago;

public sealed partial class PagoUseCase
{
    public async Task RealizarPagoAsync(int p_oid)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.UseCases_Pago_realizarPago) ENABLED START*/
        // Resolve external infrastructure services with _utils!.GetRequiredService<TService>() when this use case receives IUnitOfWorkUtils.
        PagoService pagoService = null!;

        try
        {
            await _unitOfWork.BeginTransactionAsync("PagoUseCase.RealizarPagoAsync");
            pagoService = new PagoService(_unitOfWork);

            // Iniciar transacción
            await _unitOfWork.BeginTransactionAsync("PagoUseCase.RealizarPagoAsync");

            // Buscar el pago
            var pago = await _unitOfWork.PagoRepository.ReadAsync(p_oid)
                ?? throw new ModelException("Pago no encontrado.");

            // Comprobar que el pago está pendiente
            if (pago.Estado != EstadoPago.Pendiente)
                throw new ModelException(
                    "Solo se pueden realizar pagos pendientes.");

            // Obtener la pasarela simulada
            var pasarela = _utils?.GetRequiredService<IPasarelaPago>()
                ?? throw new ModelException(
                    "No se ha configurado la pasarela de pago.");

            // Ejecutar el pago
            string idTransaccion = await pasarela.RealizarPagoAsync(
                pago.Id,
                pago.Monto.Cantidad
            );

            // Guardar el identificador de la transacción
            pago.IdTransaccionPasarela = idTransaccion;

            // Cambiar el estado del pago
            pago.Estado = EstadoPago.Completado;

            // Guardar los cambios
            await _unitOfWork.PagoRepository.SaveAsync(pago);

            // Confirmar transacción
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new ModelException("Application error while executing use case Pago.realizarPago.", ex);
        }
        /*PROTECTED REGION END*/
    }
}
