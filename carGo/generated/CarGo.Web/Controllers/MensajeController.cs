// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(MensajeController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Web.Assemblers;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

/*PROTECTED REGION ID(MensajeController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class MensajeController : Controller
{
    /*PROTECTED REGION ID(MensajeController_dependencies) START*/
    private readonly MensajeService _mensajeService;
    private readonly ILogger<MensajeController> _logger;
    private readonly CarGo.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public MensajeController(MensajeService mensajeService, CarGo.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<MensajeController>? logger = null)
    {
        _mensajeService = mensajeService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<MensajeController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(MensajeController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(MensajeController_Index_GET_body) START*/

        var entities = await _mensajeService.ReadAllAsync();
        return View(entities.Select(MensajeAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(MensajeController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(MensajeController_Details_GET_body) START*/

        var entity = await _mensajeService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(MensajeAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(MensajeController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(MensajeController_Create_GET_body) START*/

        var model = new CarGo.Web.ViewModels.MensajeViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(MensajeController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(CarGo.Web.ViewModels.MensajeViewModel model)
    {
    /*PROTECTED REGION ID(MensajeController_Create_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new MensajeService(unitOfWork);



        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ViajeId is int selectedViaje &&
            !model.ViajeOptions.Any(option => option.Value == Convert.ToString(selectedViaje, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ViajeId", "Selecciona un registro existente de Viaje.");
        if (model.ViajeId is null) ModelState.AddModelError("ViajeId", "Debes seleccionar una opción.");
        if (model.RemitenteId is int selectedRemitente &&
            !model.RemitenteOptions.Any(option => option.Value == Convert.ToString(selectedRemitente, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("RemitenteId", "Selecciona un registro existente de Usuario.");
        if (model.RemitenteId is null) ModelState.AddModelError("RemitenteId", "Debes seleccionar una opción.");


        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Mensaje.Create");
        try
        {

            var entity = await service.NewAsync(model.Contenido, model.FechaHora, model.ViajeId.GetValueOrDefault(), model.RemitenteId.GetValueOrDefault());
            entity.Viaje = model.ViajeId is int referenceViaje
                ? await unitOfWork.ViajeRepository.ReadAsync(referenceViaje) ?? throw new InvalidOperationException("El registro seleccionado de Viaje ya no existe.") : null!;
            entity.Remitente = model.RemitenteId is int referenceRemitente
                ? await unitOfWork.UsuarioRepository.ReadAsync(referenceRemitente) ?? throw new InvalidOperationException("El registro seleccionado de Usuario ya no existe.") : null!;

            await service.ModifierMensajeDefaultAsync(entity);

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
    /*PROTECTED REGION ID(MensajeController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(MensajeController_Edit_GET_body) START*/

        var entity = await _mensajeService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = MensajeAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(MensajeController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, CarGo.Web.ViewModels.MensajeViewModel model)
    {
    /*PROTECTED REGION ID(MensajeController_Edit_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new MensajeService(unitOfWork);
        var entity = await service.ReadOIDMensajeDefaultAsync(id);
        if (entity is null) return NotFound();


        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ViajeId is int selectedViaje &&
            !model.ViajeOptions.Any(option => option.Value == Convert.ToString(selectedViaje, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ViajeId", "Selecciona un registro existente de Viaje.");
        if (model.ViajeId is null) ModelState.AddModelError("ViajeId", "Debes seleccionar una opción.");
        if (model.RemitenteId is int selectedRemitente &&
            !model.RemitenteOptions.Any(option => option.Value == Convert.ToString(selectedRemitente, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("RemitenteId", "Selecciona un registro existente de Usuario.");
        if (model.RemitenteId is null) ModelState.AddModelError("RemitenteId", "Debes seleccionar una opción.");


        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Mensaje.Edit");
        try
        {

            await service.ModifyAsync(id, model.Contenido, model.FechaHora);
            entity.Viaje = model.ViajeId is int referenceViaje
                ? await unitOfWork.ViajeRepository.ReadAsync(referenceViaje) ?? throw new InvalidOperationException("El registro seleccionado de Viaje ya no existe.") : null!;
            entity.Remitente = model.RemitenteId is int referenceRemitente
                ? await unitOfWork.UsuarioRepository.ReadAsync(referenceRemitente) ?? throw new InvalidOperationException("El registro seleccionado de Usuario ya no existe.") : null!;

            await service.ModifierMensajeDefaultAsync(entity);

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
    /*PROTECTED REGION ID(MensajeController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(MensajeController_Delete_GET_body) START*/

        var entity = await _mensajeService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(MensajeAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(MensajeController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(MensajeController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new CarGo.Web.ViewModels.MensajeViewModel());
        await _mensajeService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(MensajeController_method_LoadFormOptionsAsync_CarGoWebViewModelsMensajeViewModel) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.MensajeViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(MensajeController_method_LoadFormOptionsAsync_CarGoWebViewModelsMensajeViewModel_CarGoApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.MensajeViewModel model, CarGo.Application.Abstractions.IUnitOfWork unitOfWork)
    {

        var options0 = (await unitOfWork.ViajeRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Observaciones, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options1 = (await unitOfWork.UsuarioRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.NombreCompleto, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.ViajeOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.RemitenteOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();

    }
    /*PROTECTED REGION END*/

}
