// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(VehiculoController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Web.Assemblers;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

/*PROTECTED REGION ID(VehiculoController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class VehiculoController : Controller
{
    /*PROTECTED REGION ID(VehiculoController_dependencies) START*/
    private readonly VehiculoService _vehiculoService;
    private readonly ILogger<VehiculoController> _logger;
    private readonly CarGo.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public VehiculoController(VehiculoService vehiculoService, CarGo.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<VehiculoController>? logger = null)
    {
        _vehiculoService = vehiculoService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<VehiculoController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(VehiculoController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(VehiculoController_Index_GET_body) START*/

        var entities = await _vehiculoService.ReadAllAsync();
        return View(entities.Select(VehiculoAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(VehiculoController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(VehiculoController_Details_GET_body) START*/

        var entity = await _vehiculoService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(VehiculoAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(VehiculoController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(VehiculoController_Create_GET_body) START*/

        var model = new CarGo.Web.ViewModels.VehiculoViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(VehiculoController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(CarGo.Web.ViewModels.VehiculoViewModel model)
    {
    /*PROTECTED REGION ID(VehiculoController_Create_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new VehiculoService(unitOfWork);



        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.PropietarioId is int selectedPropietario &&
            !model.PropietarioOptions.Any(option => option.Value == Convert.ToString(selectedPropietario, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("PropietarioId", "Selecciona un registro existente de Usuario.");
        if (model.PropietarioId is null) ModelState.AddModelError("PropietarioId", "Debes seleccionar una opción.");


        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Vehiculo.Create");
        try
        {

            var entity = await service.NewAsync(model.MarcaModelo, model.Matricula, model.InformacionAdicional, model.PropietarioId.GetValueOrDefault());
            entity.Propietario = model.PropietarioId is int referencePropietario
                ? await unitOfWork.UsuarioRepository.ReadAsync(referencePropietario) ?? throw new InvalidOperationException("El registro seleccionado de Usuario ya no existe.") : null!;

            await service.ModifierVehiculoDefaultAsync(entity);

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
    /*PROTECTED REGION ID(VehiculoController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(VehiculoController_Edit_GET_body) START*/

        var entity = await _vehiculoService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = VehiculoAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(VehiculoController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, CarGo.Web.ViewModels.VehiculoViewModel model)
    {
    /*PROTECTED REGION ID(VehiculoController_Edit_POST_body) START*/

        ModelState.Remove(nameof(model.InformacionAdicional)); // Campo fuera del contrato Edit de Application.
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new VehiculoService(unitOfWork);
        var entity = await service.ReadOIDVehiculoDefaultAsync(id);
        if (entity is null) return NotFound();


        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.PropietarioId is int selectedPropietario &&
            !model.PropietarioOptions.Any(option => option.Value == Convert.ToString(selectedPropietario, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("PropietarioId", "Selecciona un registro existente de Usuario.");
        if (model.PropietarioId is null) ModelState.AddModelError("PropietarioId", "Debes seleccionar una opción.");


        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Vehiculo.Edit");
        try
        {

            await service.ModifyAsync(id, model.MarcaModelo, model.Matricula);
            entity.Propietario = model.PropietarioId is int referencePropietario
                ? await unitOfWork.UsuarioRepository.ReadAsync(referencePropietario) ?? throw new InvalidOperationException("El registro seleccionado de Usuario ya no existe.") : null!;

            await service.ModifierVehiculoDefaultAsync(entity);

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
    /*PROTECTED REGION ID(VehiculoController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(VehiculoController_Delete_GET_body) START*/

        var entity = await _vehiculoService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(VehiculoAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(VehiculoController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(VehiculoController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new CarGo.Web.ViewModels.VehiculoViewModel());
        await _vehiculoService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(VehiculoController_method_LoadFormOptionsAsync_CarGoWebViewModelsVehiculoViewModel) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.VehiculoViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(VehiculoController_method_LoadFormOptionsAsync_CarGoWebViewModelsVehiculoViewModel_CarGoApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.VehiculoViewModel model, CarGo.Application.Abstractions.IUnitOfWork unitOfWork)
    {

        var options0 = (await unitOfWork.UsuarioRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.NombreCompleto, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.PropietarioOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();

    }
    /*PROTECTED REGION END*/

}
