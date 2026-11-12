// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(ViajeController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Web.Assemblers;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

/*PROTECTED REGION ID(ViajeController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class ViajeController : Controller
{
    /*PROTECTED REGION ID(ViajeController_dependencies) START*/
    private readonly ViajeService _viajeService;
    private readonly ILogger<ViajeController> _logger;
    private readonly CarGo.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public ViajeController(ViajeService viajeService, CarGo.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<ViajeController>? logger = null)
    {
        _viajeService = viajeService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<ViajeController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(ViajeController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(ViajeController_Index_GET_body) START*/

        var entities = await _viajeService.ReadAllAsync();
        return View(entities.Select(ViajeAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ViajeController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(ViajeController_Details_GET_body) START*/

        var entity = await _viajeService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(ViajeAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ViajeController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(ViajeController_Create_GET_body) START*/

        var model = new CarGo.Web.ViewModels.ViajeViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ViajeController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(CarGo.Web.ViewModels.ViajeViewModel model)
    {
    /*PROTECTED REGION ID(ViajeController_Create_POST_body) START*/

        ModelState.Remove(nameof(model.Observaciones)); // Campo fuera del contrato Create de Application.
        ModelState.Remove(nameof(model.LatitudActual)); // Campo fuera del contrato Create de Application.
        ModelState.Remove(nameof(model.LongitudActual)); // Campo fuera del contrato Create de Application.
        ModelState.Remove(nameof(model.FechaUltimaPosicion)); // Campo fuera del contrato Create de Application.
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new ViajeService(unitOfWork);



        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ConductorId is int selectedConductor &&
            !model.ConductorOptions.Any(option => option.Value == Convert.ToString(selectedConductor, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ConductorId", "Selecciona un registro existente de Usuario.");
        if (model.ConductorId is null) ModelState.AddModelError("ConductorId", "Debes seleccionar una opción.");


        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Viaje.Create");
        try
        {

            var entity = await service.NewAsync(new global::CarGo.Domain.ValueObjects.Ubicacion { Ciudad = model.OrigenCiudad, Direccion = model.OrigenDireccion, Latitud = model.OrigenLatitud, Longitud = model.OrigenLongitud }, new global::CarGo.Domain.ValueObjects.Ubicacion { Ciudad = model.DestinoCiudad, Direccion = model.DestinoDireccion, Latitud = model.DestinoLatitud, Longitud = model.DestinoLongitud }, model.FechaHoraSalida, model.NumPlazasDisponibles, new global::CarGo.Domain.ValueObjects.Dinero { Moneda = model.PrecioMoneda, Cantidad = model.PrecioCantidad }, model.PermiteMascotas, model.PermiteEquipaje, model.Estado, model.ConductorId.GetValueOrDefault());
            entity.Conductor = model.ConductorId is int referenceConductor
                ? await unitOfWork.UsuarioRepository.ReadAsync(referenceConductor) ?? throw new InvalidOperationException("El registro seleccionado de Usuario ya no existe.") : null!;

            await service.ModifierViajeDefaultAsync(entity);

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
    /*PROTECTED REGION ID(ViajeController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(ViajeController_Edit_GET_body) START*/

        var entity = await _viajeService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = ViajeAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ViajeController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, CarGo.Web.ViewModels.ViajeViewModel model)
    {
    /*PROTECTED REGION ID(ViajeController_Edit_POST_body) START*/

        ModelState.Remove(nameof(model.Observaciones)); // Campo fuera del contrato Edit de Application.
        ModelState.Remove(nameof(model.LatitudActual)); // Campo fuera del contrato Edit de Application.
        ModelState.Remove(nameof(model.LongitudActual)); // Campo fuera del contrato Edit de Application.
        ModelState.Remove(nameof(model.FechaUltimaPosicion)); // Campo fuera del contrato Edit de Application.
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new ViajeService(unitOfWork);
        var entity = await service.ReadOIDViajeDefaultAsync(id);
        if (entity is null) return NotFound();


        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ConductorId is int selectedConductor &&
            !model.ConductorOptions.Any(option => option.Value == Convert.ToString(selectedConductor, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ConductorId", "Selecciona un registro existente de Usuario.");
        if (model.ConductorId is null) ModelState.AddModelError("ConductorId", "Debes seleccionar una opción.");


        if (!ModelState.IsValid) return View(model);

        await unitOfWork.BeginTransactionAsync("MVC.Viaje.Edit");
        try
        {

            await service.ModifyAsync(id, new global::CarGo.Domain.ValueObjects.Ubicacion { Ciudad = model.OrigenCiudad, Direccion = model.OrigenDireccion, Latitud = model.OrigenLatitud, Longitud = model.OrigenLongitud }, new global::CarGo.Domain.ValueObjects.Ubicacion { Ciudad = model.DestinoCiudad, Direccion = model.DestinoDireccion, Latitud = model.DestinoLatitud, Longitud = model.DestinoLongitud }, model.FechaHoraSalida, model.NumPlazasDisponibles, new global::CarGo.Domain.ValueObjects.Dinero { Moneda = model.PrecioMoneda, Cantidad = model.PrecioCantidad }, model.PermiteMascotas, model.PermiteEquipaje, model.Estado);
            entity.Conductor = model.ConductorId is int referenceConductor
                ? await unitOfWork.UsuarioRepository.ReadAsync(referenceConductor) ?? throw new InvalidOperationException("El registro seleccionado de Usuario ya no existe.") : null!;

            await service.ModifierViajeDefaultAsync(entity);

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
    /*PROTECTED REGION ID(ViajeController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(ViajeController_Delete_GET_body) START*/

        var entity = await _viajeService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(ViajeAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ViajeController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(ViajeController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new CarGo.Web.ViewModels.ViajeViewModel());
        await _viajeService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ViajeController_AddPasajero_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> AddPasajero(int p_oid, IReadOnlyList<int> p_usuario_oids)
    {
    /*PROTECTED REGION ID(ViajeController_AddPasajero_POST_body) START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _viajeService.AddPasajeroAsync(p_oid, p_usuario_oids);
        return NoContent();
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ViajeController_RemovePasajero_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> RemovePasajero(int p_oid, IReadOnlyList<int> p_usuario_oids)
    {
    /*PROTECTED REGION ID(ViajeController_RemovePasajero_POST_body) START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _viajeService.RemovePasajeroAsync(p_oid, p_usuario_oids);
        return NoContent();
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ViajeController_ReadFilterByOrigenDestino_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ReadFilterByOrigenDestino(string p_origen, string p_destino)
    {
    /*PROTECTED REGION ID(ViajeController_ReadFilterByOrigenDestino_GET_body) START*/

        var result = await _viajeService.ReadFilterByOrigenDestinoAsync(p_origen, p_destino);
        return Ok(result.Select(ViajeAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ViajeController_ReadFilterByFechaYPlazas_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ReadFilterByFechaYPlazas(DateTime p_desde, DateTime p_hasta, int p_plazasMinimas)
    {
    /*PROTECTED REGION ID(ViajeController_ReadFilterByFechaYPlazas_GET_body) START*/

        var result = await _viajeService.ReadFilterByFechaYPlazasAsync(p_desde, p_hasta, p_plazasMinimas);
        return Ok(result.Select(ViajeAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ViajeController_ReadFilterByPrecio_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ReadFilterByPrecio(double p_precioMinimo, double p_precioMaximo)
    {
    /*PROTECTED REGION ID(ViajeController_ReadFilterByPrecio_GET_body) START*/

        var result = await _viajeService.ReadFilterByPrecioAsync(p_precioMinimo, p_precioMaximo);
        return Ok(result.Select(ViajeAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ViajeController_ReadFilterByPreferenciasYEstado_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ReadFilterByPreferenciasYEstado(bool p_permiteMascotas, bool p_permiteEquipaje)
    {
    /*PROTECTED REGION ID(ViajeController_ReadFilterByPreferenciasYEstado_GET_body) START*/

        var result = await _viajeService.ReadFilterByPreferenciasYEstadoAsync(p_permiteMascotas, p_permiteEquipaje);
        return Ok(result.Select(ViajeAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ViajeController_ReadFilterViajesDisponibles_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ReadFilterViajesDisponibles(DateTime p_fechaMinima)
    {
    /*PROTECTED REGION ID(ViajeController_ReadFilterViajesDisponibles_GET_body) START*/

        var result = await _viajeService.ReadFilterViajesDisponiblesAsync(p_fechaMinima);
        return Ok(result.Select(ViajeAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ViajeController_ReadFilterByConductorVerificado_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ReadFilterByConductorVerificado()
    {
    /*PROTECTED REGION ID(ViajeController_ReadFilterByConductorVerificado_GET_body) START*/

        var result = await _viajeService.ReadFilterByConductorVerificadoAsync();
        return Ok(result.Select(ViajeAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(ViajeController_method_LoadFormOptionsAsync_CarGoWebViewModelsViajeViewModel) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.ViajeViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ViajeController_method_LoadFormOptionsAsync_CarGoWebViewModelsViajeViewModel_CarGoApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(CarGo.Web.ViewModels.ViajeViewModel model, CarGo.Application.Abstractions.IUnitOfWork unitOfWork)
    {

        var options0 = (await unitOfWork.UsuarioRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.NombreCompleto, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.ConductorOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();

    }
    /*PROTECTED REGION END*/

}
