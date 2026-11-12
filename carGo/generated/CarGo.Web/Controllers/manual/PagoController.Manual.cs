// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(PagoController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Web.Assemblers;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION ID(PagoController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION END*/
public partial class PagoController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(PagoController_RealizarPago_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> RealizarPago(int p_oid)
    {
    /*PROTECTED REGION ID(PagoController_RealizarPago_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await using var unitOfWork = _useCaseUnitOfWorkFactory.Create();
        var useCase = new CarGo.Application.UseCases.Pago.PagoUseCase(unitOfWork);
        await useCase.RealizarPagoAsync(p_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(PagoController_ReembolsarPago_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ReembolsarPago(int p_oid)
    {
    /*PROTECTED REGION ID(PagoController_ReembolsarPago_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await using var unitOfWork = _useCaseUnitOfWorkFactory.Create();
        var useCase = new CarGo.Application.UseCases.Pago.PagoUseCase(unitOfWork);
        await useCase.ReembolsarPagoAsync(p_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }
}
