// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(NotificacionController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Web.Assemblers;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

/*PROTECTED REGION ID(NotificacionController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class NotificacionController : Controller
{
    /*PROTECTED REGION ID(NotificacionController_dependencies) START*/
    private readonly NotificacionService _notificacionService;
    private readonly ILogger<NotificacionController> _logger;
    private readonly CarGo.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public NotificacionController(NotificacionService notificacionService, CarGo.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<NotificacionController>? logger = null)
    {
        _notificacionService = notificacionService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<NotificacionController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(NotificacionController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(NotificacionController_Index_GET_body) START*/

        var entities = await _notificacionService.ReadAllAsync();
        return View(entities.Select(NotificacionAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(NotificacionController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(NotificacionController_Details_GET_body) START*/

        var entity = await _notificacionService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(NotificacionAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(NotificacionController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(NotificacionController_Create_GET_body) START*/

        var model = new CarGo.Web.ViewModels.NotificacionViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(NotificacionController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(CarGo.Web.ViewModels.NotificacionViewModel model)
    {
    /*PROTECTED REGION ID(NotificacionController_Create_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new NotificacionService(unitOfWork);



        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.UsuarioNotificadoId is int selectedUsuarioNotificado &&
            !model.UsuarioNotificadoOptions.Any(option => option.Value == Convert.ToString(selectedUsuarioNotificado, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("UsuarioNotificadoId", "Selecciona un registro existente de Usuario.");
        if (model.UsuarioNotificadoId is null) ModelState.AddModelError("UsuarioNotificadoId", "Debes seleccionar una opción.");


        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Notificacion.Create");
        try
        {

            var entity = await service.NewAsync(model.Titulo, model.Texto, model.FechaHora, model.Leida, model.UsuarioNotificadoId.GetValueOrDefault());
            entity.UsuarioNotificado = model.UsuarioNotificadoId is int referenceUsuarioNotificado
                ? await unitOfWork.UsuarioRepository.ReadAsync(referenceUsuarioNotificado) ?? throw new InvalidOperationException("El registro seleccionado de Usuario ya no existe.") : null!;

            await service.ModifierNotificacionDefaultAsync(entity);

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
    /*PROTECTED REGION ID(NotificacionController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(NotificacionController_Edit_GET_body) START*/

        var entity = await _notificacionService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = NotificacionAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(NotificacionController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, CarGo.Web.ViewModels.NotificacionViewModel model)
    {
    /*PROTECTED REGION ID(NotificacionController_Edit_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new NotificacionService(unitOfWork);
        var entity = await service.ReadOIDNotificacionDefaultAsync(id);
        if (entity is null) return NotFound();


        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.UsuarioNotificadoId is int selectedUsuarioNotificado &&
            !model.UsuarioNotificadoOptions.Any(option => option.Value == Convert.ToString(selectedUsuarioNotificado, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("UsuarioNotificadoId", "Selecciona un registro existente de Usuario.");
        if (model.UsuarioNotificadoId is null) ModelState.AddModelError("UsuarioNotificadoId", "Debes seleccionar una opción.");


        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Notificacion.Edit");
        try
        {

            await service.ModifyAsync(id, model.Titulo, model.Texto, model.FechaHora, model.Leida, model.UsuarioNotificadoId.GetValueOrDefault());
            entity.UsuarioNotificado = model.UsuarioNotificadoId is int referenceUsuarioNotificado
                ? await unitOfWork.UsuarioRepository.ReadAsync(referenceUsuarioNotificado) ?? throw new InvalidOperationException("El registro seleccionado de Usuario ya no existe.") : null!;

            await service.ModifierNotificacionDefaultAsync(entity);

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
    /*PROTECTED REGION ID(NotificacionController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(NotificacionController_Delete_GET_body) START*/

        var entity = await _notificacionService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(NotificacionAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(NotificacionController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(NotificacionController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new CarGo.Web.ViewModels.NotificacionViewModel());
        await _notificacionService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(NotificacionController_method_LoadFormOptionsAsync_CarGoWebViewModelsNotificacionViewModel) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.NotificacionViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(NotificacionController_method_LoadFormOptionsAsync_CarGoWebViewModelsNotificacionViewModel_CarGoApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.NotificacionViewModel model, CarGo.Application.Abstractions.IUnitOfWork unitOfWork)
    {

        var options0 = (await unitOfWork.UsuarioRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.NombreCompleto, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.UsuarioNotificadoOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();

    }
    /*PROTECTED REGION END*/

}
