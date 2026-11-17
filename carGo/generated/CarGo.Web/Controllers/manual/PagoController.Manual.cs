// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(PagoController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Web.Assemblers;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
using CarGo.Application.Exceptions;
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

        try
        {
<<<<<<< Updated upstream
            await using var unitOfWork = _useCaseUnitOfWorkFactory.Create(); 
=======
            await using var unitOfWork = _useCaseUnitOfWorkFactory.Create(); //abre una sesion de trabajo para el caso de uso
            // La pasarela de pago se registra aquí para no depender de Program.cs (que HARIA regenera)
            var servicios = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            CarGo.Infrastructure.Utils.Manual.ExternalUtilRegistration.Register(
                servicios,
                Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>(HttpContext.RequestServices));
            using var proveedor = Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicios);
            var utils = new CarGo.Infrastructure.Utils.Generated.UnitOfWorkUtils(proveedor);
            var useCase = new CarGo.Application.UseCases.Pago.PagoUseCase(unitOfWork, utils);
            await useCase.RealizarPagoAsync(p_oid);
            TempData["Exito"] = "Pago realizado correctamente.";
        }
        catch (ModelException ex)
        {
            TempData["Error"] = ex.Message;
        }

    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(PagoController_ReembolsarPago_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ReembolsarPago(int p_oid)
    {
    /*PROTECTED REGION ID(PagoController_ReembolsarPago_POST_body) ENABLED START*/

        try
        {
            await using var unitOfWork = _useCaseUnitOfWorkFactory.Create();
            // La pasarela de pago se registra aquí para no depender de Program.cs (que HARIA regenera)
            var servicios = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            CarGo.Infrastructure.Utils.Manual.ExternalUtilRegistration.Register(
                servicios,
                Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>(HttpContext.RequestServices));
            using var proveedor = Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(servicios);
            var utils = new CarGo.Infrastructure.Utils.Generated.UnitOfWorkUtils(proveedor);
            var useCase = new CarGo.Application.UseCases.Pago.PagoUseCase(unitOfWork, utils);
            await useCase.ReembolsarPagoAsync(p_oid);
            TempData["Exito"] = "Pago reembolsado.";
        }
        catch (ModelException ex)
        {
            TempData["Error"] = ex.Message;
        }

    /*PROTECTED REGION END*/
    }
}
