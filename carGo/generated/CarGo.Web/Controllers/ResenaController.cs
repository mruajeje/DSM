// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(ResenaController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Web.Assemblers;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

/*PROTECTED REGION ID(ResenaController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class ResenaController : Controller
{
    /*PROTECTED REGION ID(ResenaController_dependencies) START*/
    private readonly ResenaService _resenaService;
    private readonly ILogger<ResenaController> _logger;
    private readonly CarGo.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public ResenaController(ResenaService resenaService, CarGo.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<ResenaController>? logger = null)
    {
        _resenaService = resenaService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<ResenaController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(ResenaController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(ResenaController_Index_GET_body) START*/

        var entities = await _resenaService.ReadAllAsync();
        return View(entities.Select(ResenaAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ResenaController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(ResenaController_Details_GET_body) START*/

        var entity = await _resenaService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(ResenaAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ResenaController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(ResenaController_Create_GET_body) START*/

        var model = new CarGo.Web.ViewModels.ResenaViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ResenaController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(CarGo.Web.ViewModels.ResenaViewModel model)
    {
    /*PROTECTED REGION ID(ResenaController_Create_POST_body) START*/

        ModelState.Remove(nameof(model.MotivoModeracion)); // Campo fuera del contrato Create de Application.
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new ResenaService(unitOfWork);



        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ViajeId is int selectedViaje &&
            !model.ViajeOptions.Any(option => option.Value == Convert.ToString(selectedViaje, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ViajeId", "Selecciona un registro existente de Viaje.");
        if (model.ViajeId is null) ModelState.AddModelError("ViajeId", "Debes seleccionar una opción.");
        if (model.AutorId is int selectedAutor &&
            !model.AutorOptions.Any(option => option.Value == Convert.ToString(selectedAutor, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("AutorId", "Selecciona un registro existente de Usuario.");
        if (model.AutorId is null) ModelState.AddModelError("AutorId", "Debes seleccionar una opción.");


        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Resena.Create");
        try
        {

            var entity = await service.NewAsync(model.Puntuacion, model.Comentario, model.FechaCreacion, model.Estado, model.ViajeId.GetValueOrDefault(), model.AutorId.GetValueOrDefault());
            entity.Viaje = model.ViajeId is int referenceViaje
                ? await unitOfWork.ViajeRepository.ReadAsync(referenceViaje) ?? throw new InvalidOperationException("El registro seleccionado de Viaje ya no existe.") : null!;
            entity.Autor = model.AutorId is int referenceAutor
                ? await unitOfWork.UsuarioRepository.ReadAsync(referenceAutor) ?? throw new InvalidOperationException("El registro seleccionado de Usuario ya no existe.") : null!;

            await service.ModifierResenaDefaultAsync(entity);

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
    /*PROTECTED REGION ID(ResenaController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(ResenaController_Edit_GET_body) START*/

        var entity = await _resenaService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = ResenaAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ResenaController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, CarGo.Web.ViewModels.ResenaViewModel model)
    {
    /*PROTECTED REGION ID(ResenaController_Edit_POST_body) START*/

        ModelState.Remove(nameof(model.Comentario)); // Campo fuera del contrato Edit de Application.
        ModelState.Remove(nameof(model.MotivoModeracion)); // Campo fuera del contrato Edit de Application.
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new ResenaService(unitOfWork);
        var entity = await service.ReadOIDResenaDefaultAsync(id);
        if (entity is null) return NotFound();


        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ViajeId is int selectedViaje &&
            !model.ViajeOptions.Any(option => option.Value == Convert.ToString(selectedViaje, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ViajeId", "Selecciona un registro existente de Viaje.");
        if (model.ViajeId is null) ModelState.AddModelError("ViajeId", "Debes seleccionar una opción.");
        if (model.AutorId is int selectedAutor &&
            !model.AutorOptions.Any(option => option.Value == Convert.ToString(selectedAutor, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("AutorId", "Selecciona un registro existente de Usuario.");
        if (model.AutorId is null) ModelState.AddModelError("AutorId", "Debes seleccionar una opción.");


        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Resena.Edit");
        try
        {

            await service.ModifyAsync(id, model.Puntuacion, model.FechaCreacion, model.Estado);
            entity.Viaje = model.ViajeId is int referenceViaje
                ? await unitOfWork.ViajeRepository.ReadAsync(referenceViaje) ?? throw new InvalidOperationException("El registro seleccionado de Viaje ya no existe.") : null!;
            entity.Autor = model.AutorId is int referenceAutor
                ? await unitOfWork.UsuarioRepository.ReadAsync(referenceAutor) ?? throw new InvalidOperationException("El registro seleccionado de Usuario ya no existe.") : null!;

            await service.ModifierResenaDefaultAsync(entity);

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
    /*PROTECTED REGION ID(ResenaController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(ResenaController_Delete_GET_body) START*/

        var entity = await _resenaService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(ResenaAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ResenaController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(ResenaController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new CarGo.Web.ViewModels.ResenaViewModel());
        await _resenaService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(ResenaController_method_LoadFormOptionsAsync_CarGoWebViewModelsResenaViewModel) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.ResenaViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ResenaController_method_LoadFormOptionsAsync_CarGoWebViewModelsResenaViewModel_CarGoApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.ResenaViewModel model, CarGo.Application.Abstractions.IUnitOfWork unitOfWork)
    {

        var options0 = (await unitOfWork.ViajeRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Observaciones, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options1 = (await unitOfWork.UsuarioRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.NombreCompleto, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.ViajeOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.AutorOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();

    }
    /*PROTECTED REGION END*/

}
