// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(UsuarioController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Web.Assemblers;
using CarGo.Web.Utils.Security;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

/*PROTECTED REGION ID(UsuarioController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class UsuarioController : Controller
{
    /*PROTECTED REGION ID(UsuarioController_dependencies) START*/
    private readonly UsuarioService _usuarioService;
    private readonly ILogger<UsuarioController> _logger;
    public UsuarioController(UsuarioService usuarioService, ILogger<UsuarioController>? logger = null)
    {
        _usuarioService = usuarioService;



        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<UsuarioController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(UsuarioController_Index_GET_body) START*/

        var entities = await _usuarioService.ReadAllAsync();
        return View(entities.Select(UsuarioAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(UsuarioController_Details_GET_body) START*/

        var entity = await _usuarioService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(UsuarioAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public IActionResult Create()
    {
    /*PROTECTED REGION ID(UsuarioController_Create_GET_body) START*/

        return View(new CarGo.Web.ViewModels.UsuarioViewModel());
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(UsuarioController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(CarGo.Web.ViewModels.UsuarioViewModel model)
    {
    /*PROTECTED REGION ID(UsuarioController_Create_POST_body) START*/

        ModelState.Remove(nameof(model.Telefono)); // Campo fuera del contrato Create de Application.
        ModelState.Remove(nameof(model.Dni)); // Campo fuera del contrato Create de Application.
        if (string.IsNullOrEmpty(model.Password)) ModelState.AddModelError(nameof(model.Password), "Debes indicar una contraseña.");
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _usuarioService.NewAsync(model.NombreCompleto, model.Email, model.Password!, model.EsAdmin, model.EstadoVerificacion);
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            _logger.LogError(error, "Could not create the record.");
            ModelState.AddModelError(string.Empty, "No se ha podido guardar el registro. Inténtalo de nuevo.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Register_GET_attributes) START*/
    [AllowAnonymous]
    /*PROTECTED REGION END*/
    public IActionResult Register()
    {
    /*PROTECTED REGION ID(UsuarioController_Register_GET_body) START*/

        return View(new CarGo.Web.ViewModels.UsuarioViewModel());
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(UsuarioController_Register_POST_attributes) START*/
    [AllowAnonymous]
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Register(CarGo.Web.ViewModels.UsuarioViewModel model)
    {
    /*PROTECTED REGION ID(UsuarioController_Register_POST_body) START*/

        ModelState.Remove(nameof(model.Telefono)); // Campo fuera del contrato Create de Application.
        ModelState.Remove(nameof(model.Dni)); // Campo fuera del contrato Create de Application.
        if (string.IsNullOrEmpty(model.Password)) ModelState.AddModelError(nameof(model.Password), "Debes indicar una contraseña.");
        if (!ModelState.IsValid) return View(model);
        Usuario createdUser;
        try
        {
            createdUser = await _usuarioService.NewAsync(model.NombreCompleto, model.Email, model.Password!, model.EsAdmin, model.EstadoVerificacion);
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            _logger.LogError(error, "Could not create the record.");
            ModelState.AddModelError(string.Empty, "No se ha podido guardar el registro. Inténtalo de nuevo.");
            return View(model);
        }
        try
        {
            await HariaWebAuthentication.SignInAsync(HttpContext, "Usuario", createdUser.Id, createdUser);
            HttpContext.Session.SetUser(UsuarioAssembler.ToViewModel(createdUser));
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            _logger.LogError(error, "La cuenta se ha creado, pero no se ha podido iniciar la sesión.");
            ModelState.AddModelError(string.Empty, "Tu cuenta ya está creada. Inicia sesión desde la página de acceso.");
            return View(model);
        }
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(UsuarioController_Edit_GET_body) START*/

        var entity = await _usuarioService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(UsuarioAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(UsuarioController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, CarGo.Web.ViewModels.UsuarioViewModel model)
    {
    /*PROTECTED REGION ID(UsuarioController_Edit_POST_body) START*/

        ModelState.Remove(nameof(model.Telefono)); // Campo fuera del contrato Edit de Application.
        ModelState.Remove(nameof(model.Dni)); // Campo fuera del contrato Edit de Application.
        if (!ModelState.IsValid) return View(model);
        var stored = await _usuarioService.ReadOIDAsync(id);
        if (stored is null) return NotFound();
        try
        {
            await _usuarioService.ModifyAsync(id, model.NombreCompleto, model.Email, (string.IsNullOrEmpty(model.Password) ? stored.Password : model.Password), model.EsAdmin, model.EstadoVerificacion);
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            _logger.LogError(error, "Could not update the record.");
            ModelState.AddModelError(string.Empty, "No se ha podido guardar el registro. Inténtalo de nuevo.");
            return View(model);
        }
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(UsuarioController_Delete_GET_body) START*/

        var entity = await _usuarioService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(UsuarioAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(UsuarioController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(UsuarioController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new CarGo.Web.ViewModels.UsuarioViewModel());
        await _usuarioService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_ReadFilterByEmail_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ReadFilterByEmail(string p_email)
    {
    /*PROTECTED REGION ID(UsuarioController_ReadFilterByEmail_GET_body) START*/

        var result = await _usuarioService.ReadFilterByEmailAsync(p_email);
        return Ok(result.Select(UsuarioAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_ReadFilterByDni_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ReadFilterByDni(string p_dni)
    {
    /*PROTECTED REGION ID(UsuarioController_ReadFilterByDni_GET_body) START*/

        var result = await _usuarioService.ReadFilterByDniAsync(p_dni);
        return Ok(result.Select(UsuarioAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }
}
