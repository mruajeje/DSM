using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Application.Services;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGo.Application.UseCases_Pago_realizarPago) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGo.Application.UseCases.Pago;

public sealed partial class PagoUseCase
{
    public async Task RealizarPagoAsync(int p_oid)
    {
        /*PROTECTED REGION ID(CarGo.Application.UseCases_Pago_realizarPago) ENABLED START*/
        // Resolve external infrastructure services with _utils!.GetRequiredService<TService>() when this use case receives IUnitOfWorkUtils.
        PagoService pagoService = null!;

        try
        {
            await _unitOfWork.BeginTransactionAsync("PagoUseCase.RealizarPagoAsync");
            pagoService = new PagoService(_unitOfWork);

            // Write here your custom transaction code...
            // Use pagoService, _unitOfWork.PagoRepository or _utils!.GetRequiredService<TService>() as the generated CP equivalent.

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
