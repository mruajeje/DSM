// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(ReservaController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Web.Assemblers;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

/*PROTECTED REGION ID(ReservaController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class ReservaController : Controller
{
    /*PROTECTED REGION ID(ReservaController_dependencies) START*/
    private readonly ViajeService _viajeService;
    private readonly ILogger<ReservaController> _logger;
    private readonly CarGo.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public ReservaController(ViajeService viajeService, CarGo.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<ReservaController>? logger = null)
    {
        _viajeService = viajeService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<ReservaController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(ReservaController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(ReservaController_Index_GET_body) START*/

        var entities = await _viajeService.Reserva_readAllAsync();
        return View(entities.Select(ReservaAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ReservaController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(ReservaController_Details_GET_body) START*/

        var entity = await _viajeService.Reserva_readOIDAsync(id);
        if (entity is null) return NotFound();
        return View(ReservaAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ReservaController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(ReservaController_Create_GET_body) START*/

        var model = new CarGo.Web.ViewModels.ReservaViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ReservaController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(CarGo.Web.ViewModels.ReservaViewModel model)
    {
    /*PROTECTED REGION ID(ReservaController_Create_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new ViajeService(unitOfWork);



        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ViajeId is int selectedViaje &&
            !model.ViajeOptions.Any(option => option.Value == Convert.ToString(selectedViaje, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ViajeId", "Selecciona un registro existente de Viaje.");
        if (model.ViajeId is null) ModelState.AddModelError("ViajeId", "Debes seleccionar una opción.");
        if (model.PasajeroId is int selectedPasajero &&
            !model.PasajeroOptions.Any(option => option.Value == Convert.ToString(selectedPasajero, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("PasajeroId", "Selecciona un registro existente de Usuario.");
        if (model.PasajeroId is null) ModelState.AddModelError("PasajeroId", "Debes seleccionar una opción.");
        if (model.PagoId is int selectedPago &&
            !model.PagoOptions.Any(option => option.Value == Convert.ToString(selectedPago, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("PagoId", "Selecciona un registro existente de Pago.");



        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Reserva.Create");
        try
        {

            var entity = await service.Reserva_newAsync(model.NumPlazasReservadas, model.Estado, model.FechaSolicitud, model.ViajeId.GetValueOrDefault(), model.PasajeroId.GetValueOrDefault());
            entity.Viaje = model.ViajeId is int referenceViaje
                ? await unitOfWork.ViajeRepository.ReadAsync(referenceViaje) ?? throw new InvalidOperationException("El registro seleccionado de Viaje ya no existe.") : null!;
            entity.Pasajero = model.PasajeroId is int referencePasajero
                ? await unitOfWork.UsuarioRepository.ReadAsync(referencePasajero) ?? throw new InvalidOperationException("El registro seleccionado de Usuario ya no existe.") : null!;
            entity.Pago = model.PagoId is int referencePago
                ? await unitOfWork.PagoRepository.ReadAsync(referencePago) ?? throw new InvalidOperationException("El registro seleccionado de Pago ya no existe.") : null!;

            await unitOfWork.ReservaRepository.SaveAsync(entity);

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
    /*PROTECTED REGION ID(ReservaController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(ReservaController_Edit_GET_body) START*/

        var entity = await _viajeService.Reserva_readOIDAsync(id);
        if (entity is null) return NotFound();
        var model = ReservaAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ReservaController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, CarGo.Web.ViewModels.ReservaViewModel model)
    {
    /*PROTECTED REGION ID(ReservaController_Edit_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new ViajeService(unitOfWork);
        var entity = await unitOfWork.ReservaRepository.ReadAsync(id);
        if (entity is null) return NotFound();


        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ViajeId is int selectedViaje &&
            !model.ViajeOptions.Any(option => option.Value == Convert.ToString(selectedViaje, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ViajeId", "Selecciona un registro existente de Viaje.");
        if (model.ViajeId is null) ModelState.AddModelError("ViajeId", "Debes seleccionar una opción.");
        if (model.PasajeroId is int selectedPasajero &&
            !model.PasajeroOptions.Any(option => option.Value == Convert.ToString(selectedPasajero, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("PasajeroId", "Selecciona un registro existente de Usuario.");
        if (model.PasajeroId is null) ModelState.AddModelError("PasajeroId", "Debes seleccionar una opción.");
        if (model.PagoId is int selectedPago &&
            !model.PagoOptions.Any(option => option.Value == Convert.ToString(selectedPago, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("PagoId", "Selecciona un registro existente de Pago.");



        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Reserva.Edit");
        try
        {

            await service.Reserva_modifyAsync(id, model.NumPlazasReservadas, model.Estado, model.FechaSolicitud);
            entity.Viaje = model.ViajeId is int referenceViaje
                ? await unitOfWork.ViajeRepository.ReadAsync(referenceViaje) ?? throw new InvalidOperationException("El registro seleccionado de Viaje ya no existe.") : null!;
            entity.Pasajero = model.PasajeroId is int referencePasajero
                ? await unitOfWork.UsuarioRepository.ReadAsync(referencePasajero) ?? throw new InvalidOperationException("El registro seleccionado de Usuario ya no existe.") : null!;
            entity.Pago = model.PagoId is int referencePago
                ? await unitOfWork.PagoRepository.ReadAsync(referencePago) ?? throw new InvalidOperationException("El registro seleccionado de Pago ya no existe.") : null!;

            await unitOfWork.ReservaRepository.SaveAsync(entity);

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
    /*PROTECTED REGION ID(ReservaController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(ReservaController_Delete_GET_body) START*/

        var entity = await _viajeService.Reserva_readOIDAsync(id);
        if (entity is null) return NotFound();
        return View(ReservaAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ReservaController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(ReservaController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new CarGo.Web.ViewModels.ReservaViewModel());
        await _viajeService.Reserva_destroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ReservaController_ReadFilterByEstadoYFecha_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ReadFilterByEstadoYFecha(EstadoReserva p_estado, DateTime p_desde, DateTime p_hasta)
    {
    /*PROTECTED REGION ID(ReservaController_ReadFilterByEstadoYFecha_GET_body) START*/

        var result = await _viajeService.Reserva_readFilterByEstadoYFechaAsync(p_estado, p_desde, p_hasta);
        return Ok(result.Select(ReservaAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(ReservaController_method_LoadFormOptionsAsync_CarGoWebViewModelsReservaViewModel) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.ReservaViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ReservaController_method_LoadFormOptionsAsync_CarGoWebViewModelsReservaViewModel_CarGoApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.ReservaViewModel model, CarGo.Application.Abstractions.IUnitOfWork unitOfWork)
    {

        var options0 = (await unitOfWork.ViajeRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Observaciones, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options1 = (await unitOfWork.UsuarioRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.NombreCompleto, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options2 = (await unitOfWork.PagoRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.IdTransaccionPasarela, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.ViajeOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.PasajeroOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.PagoOptions = options2.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();

    }
    /*PROTECTED REGION END*/

}
