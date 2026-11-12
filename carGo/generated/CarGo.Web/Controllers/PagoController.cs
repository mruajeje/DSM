// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(PagoController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Web.Assemblers;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

/*PROTECTED REGION ID(PagoController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class PagoController : Controller
{
    /*PROTECTED REGION ID(PagoController_dependencies) START*/
    private readonly CarGo.Application.Abstractions.IUnitOfWorkFactory _useCaseUnitOfWorkFactory;
    private readonly PagoService _pagoService;
    private readonly ILogger<PagoController> _logger;
    private readonly CarGo.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public PagoController(PagoService pagoService, CarGo.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, CarGo.Application.Abstractions.IUnitOfWorkFactory useCaseUnitOfWorkFactory, ILogger<PagoController>? logger = null)
    {
        _pagoService = pagoService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;
        _useCaseUnitOfWorkFactory = useCaseUnitOfWorkFactory;
        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<PagoController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(PagoController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(PagoController_Index_GET_body) START*/

        var entities = await _pagoService.ReadAllAsync();
        return View(entities.Select(PagoAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(PagoController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(PagoController_Details_GET_body) START*/

        var entity = await _pagoService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(PagoAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(PagoController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(PagoController_Create_GET_body) START*/

        var model = new CarGo.Web.ViewModels.PagoViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(PagoController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(CarGo.Web.ViewModels.PagoViewModel model)
    {
    /*PROTECTED REGION ID(PagoController_Create_POST_body) START*/

        ModelState.Remove(nameof(model.IdTransaccionPasarela)); // Campo fuera del contrato Create de Application.
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new PagoService(unitOfWork);



        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ReservaId is int selectedReserva &&
            !model.ReservaOptions.Any(option => option.Value == Convert.ToString(selectedReserva, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ReservaId", "Selecciona un registro existente de Reserva.");
        if (model.ReservaId is null) ModelState.AddModelError("ReservaId", "Debes seleccionar una opción.");


        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Pago.Create");
        try
        {

            var entity = await service.NewAsync(new global::CarGo.Domain.ValueObjects.Dinero { Moneda = model.MontoMoneda, Cantidad = model.MontoCantidad }, model.FechaPago, model.Estado, model.ReservaId.GetValueOrDefault());
            entity.Reserva = model.ReservaId is int referenceReserva
                ? await unitOfWork.ReservaRepository.ReadAsync(referenceReserva) ?? throw new InvalidOperationException("El registro seleccionado de Reserva ya no existe.") : null!;

            await service.ModifierPagoDefaultAsync(entity);

            await unitOfWork.CommitAsync();
        }
        catch (Exception error)
        {
            await unitOfWork.RollbackAsync();
            if (error is OperationCanceledException) throw;
            _logger.LogError(error, "Could not save the record and its lines.");
            ModelState.AddModelError(string.Empty, "No se han podido guardar el registro y sus líneas.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(PagoController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(PagoController_Edit_GET_body) START*/

        var entity = await _pagoService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = PagoAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(PagoController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, CarGo.Web.ViewModels.PagoViewModel model)
    {
    /*PROTECTED REGION ID(PagoController_Edit_POST_body) START*/

        ModelState.Remove(nameof(model.IdTransaccionPasarela)); // Campo fuera del contrato Edit de Application.
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new PagoService(unitOfWork);
        var entity = await service.ReadOIDPagoDefaultAsync(id);
        if (entity is null) return NotFound();


        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ReservaId is int selectedReserva &&
            !model.ReservaOptions.Any(option => option.Value == Convert.ToString(selectedReserva, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ReservaId", "Selecciona un registro existente de Reserva.");
        if (model.ReservaId is null) ModelState.AddModelError("ReservaId", "Debes seleccionar una opción.");


        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Pago.Edit");
        try
        {

            await service.ModifyAsync(id, new global::CarGo.Domain.ValueObjects.Dinero { Moneda = model.MontoMoneda, Cantidad = model.MontoCantidad }, model.FechaPago, model.Estado);
            entity.Reserva = model.ReservaId is int referenceReserva
                ? await unitOfWork.ReservaRepository.ReadAsync(referenceReserva) ?? throw new InvalidOperationException("El registro seleccionado de Reserva ya no existe.") : null!;

            await service.ModifierPagoDefaultAsync(entity);

            await unitOfWork.CommitAsync();
        }
        catch (Exception error)
        {
            await unitOfWork.RollbackAsync();
            if (error is OperationCanceledException) throw;
            _logger.LogError(error, "Could not save the record and its lines.");
            ModelState.AddModelError(string.Empty, "No se han podido guardar el registro y sus líneas.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(PagoController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(PagoController_Delete_GET_body) START*/

        var entity = await _pagoService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(PagoAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(PagoController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(PagoController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new CarGo.Web.ViewModels.PagoViewModel());
        await _pagoService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(PagoController_method_LoadFormOptionsAsync_CarGoWebViewModelsPagoViewModel) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.PagoViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(PagoController_method_LoadFormOptionsAsync_CarGoWebViewModelsPagoViewModel_CarGoApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.PagoViewModel model, CarGo.Application.Abstractions.IUnitOfWork unitOfWork)
    {

        var options0 = (await unitOfWork.ReservaRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.ReservaOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();

    }
    /*PROTECTED REGION END*/

}
