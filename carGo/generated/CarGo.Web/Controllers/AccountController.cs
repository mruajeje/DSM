using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarGo.Application.Abstractions;
using CarGo.Web.Assemblers;
using CarGo.Web.Utils.Security;
using CarGo.Web.ViewModels;

namespace CarGo.Web.Controllers;

public sealed class AccountController : Controller
{
    private readonly CarGo.Application.Services.UsuarioService _userService;
    private readonly IUnitOfWorkFactory _userRepository;
    private readonly IHariaPasswordHasher _passwordHasher;

    public AccountController(CarGo.Application.Services.UsuarioService userService,
        IUnitOfWorkFactory userRepository,
        IHariaPasswordHasher passwordHasher)
    {
        _userService = userService;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null) =>
        View(new LoginViewModel { ReturnUrl = returnUrl });

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim();

        // La entidad se carga dentro del UnitOfWork para que AddTrustedClaims pueda leer los vehículos
        await using var unitOfWork = _userRepository.Create();
        var usuarios = await unitOfWork.UsuarioRepository.ReadFilterByEmailAsync(email);
        var entity = usuarios.FirstOrDefault();

        // Mismo mensaje si falla el email o la contraseña, para no dar pistas
        if (entity is null || !_passwordHasher.VerifyPassword(entity.Password, model.Password))
        {
            ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
            return View(model);
        }
        var user = UsuarioAssembler.ToViewModel(entity);

        await HariaWebAuthentication.SignInAsync(HttpContext, "Usuario", entity.Id, entity);
        HttpContext.Session.SetUser(user);

        return Url.IsLocalUrl(model.ReturnUrl)
            ? LocalRedirect(model.ReturnUrl!)
            : RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.ClearClasses();
        return RedirectToAction("Login");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied() => View();
}