using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Application.Services;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGo.Application.UseCases_Pago_reembolsarPago) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGo.Application.UseCases.Pago;

public sealed partial class PagoUseCase
{
    public async Task ReembolsarPagoAsync(int p_oid)
    {
        /*PROTECTED REGION ID(CarGo.Application.UseCases_Pago_reembolsarPago) ENABLED START*/
        // Resolve external infrastructure services with _utils!.GetRequiredService<TService>() when this use case receives IUnitOfWorkUtils.
        PagoService pagoService = null!;

        try
        {
            await _unitOfWork.BeginTransactionAsync("PagoUseCase.ReembolsarPagoAsync");
            pagoService = new PagoService(_unitOfWork);

            // Iniciar transacción
            await _unitOfWork.BeginTransactionAsync(
                "PagoUseCase.ReembolsarPagoAsync");

            // Buscar el pago
            var pago = await _unitOfWork.PagoRepository.ReadAsync(p_oid)
                ?? throw new ModelException("Pago no encontrado.");

            // Comprobar que está completado
            if (pago.Estado != EstadoPago.Completado)
                throw new ModelException(
                    "Solo se pueden reembolsar pagos completados.");

            // Comprobar que tiene una transacción asociada
            if (string.IsNullOrWhiteSpace(pago.IdTransaccionPasarela))
                throw new ModelException(
                    "El pago no tiene una transacción asociada.");

             // Obtener la pasarela simulada
            var pasarela = _utils?.GetRequiredService<IPasarelaPago>()
                ?? throw new ModelException(
                    "No se ha configurado la pasarela de pago.");

            // Ejecutar el reembolso
            await pasarela.ReembolsarPagoAsync(
                pago.IdTransaccionPasarela);

            // Actualizar el estado
            pago.Estado = EstadoPago.Reembolsado;

            // Guardar los cambios
            await _unitOfWork.PagoRepository.SaveAsync(pago);

            // Confirmar transacción
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new ModelException("Application error while executing use case Pago.reembolsarPago.", ex);
        }
        /*PROTECTED REGION END*/
    }
}
