// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(UsuarioController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Application.Exceptions;
using CarGo.Web.Assemblers;
using CarGo.Web.Utils.Security;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION ID(UsuarioController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION END*/
public partial class UsuarioController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(UsuarioController_VerificarDni_POST_attributes) ENABLED START*/
    [Authorize(Roles = "Pasajero")]
    /*PROTECTED REGION END*/
    public async Task<IActionResult> VerificarDni(int p_oid, string p_dni)
    {
    /*PROTECTED REGION ID(UsuarioController_VerificarDni_POST_body) ENABLED START*/

        // El usuario solo puede verificar SU propio DNI: el id sale de la cookie, no del formulario
        if (!int.TryParse(User.Identity?.Name, out var usuarioId)) return Forbid();

        try
        {
            await _usuarioService.VerificarDniAsync(usuarioId, p_dni);
            TempData["Exito"] = "DNI registrado. Queda pendiente de revisión.";
        }
        catch (ModelException error)
        {
            TempData["Error"] = error.Message;
        }
        return RedirectToAction(nameof(Details), new { id = usuarioId });
    /*PROTECTED REGION END*/
    }

    // Buscador de usuarios por email y/o DNI (solo admin). Reutiliza la vista Index.
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Buscar(string? email, string? dni)
    {
        var emailBuscado = email?.Trim();
        var dniBuscado = dni?.Trim().ToUpperInvariant();

        IEnumerable<Usuario> resultado;
        if (!string.IsNullOrEmpty(emailBuscado))
        {
            resultado = await _usuarioService.ReadFilterByEmailAsync(emailBuscado);
            if (!string.IsNullOrEmpty(dniBuscado))
            {
                resultado = resultado.Where(u => string.Equals(u.Dni, dniBuscado, StringComparison.OrdinalIgnoreCase));
            }
        }
        else if (!string.IsNullOrEmpty(dniBuscado))
        {
            resultado = await _usuarioService.ReadFilterByDniAsync(dniBuscado);
        }
        else
        {
            resultado = await _usuarioService.ReadAllAsync();
        }

        ViewData["Email"] = emailBuscado;
        ViewData["Dni"] = dniBuscado;
        return View("Index", resultado.Select(UsuarioAssembler.ToViewModel).ToList());
    }

    // Subida de foto de perfil del usuario autenticado (o de cualquier usuario si es Admin)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubirFotoPerfil(int p_oid, IFormFile? foto)
    {
        if (!int.TryParse(User.Identity?.Name, out var usuarioSesionId)) return Forbid();

        var destinoId = User.IsInRole("Admin") && p_oid > 0 ? p_oid : usuarioSesionId;

        if (foto == null || foto.Length == 0)
        {
            TempData["Error"] = "Selecciona una imagen válida para tu perfil.";
            return RedirectToAction(nameof(Details), new { id = destinoId });
        }

        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(foto.FileName).ToLowerInvariant();
        if (!extensionesPermitidas.Contains(ext))
        {
            TempData["Error"] = "Formato no permitido. Usa JPG, PNG o WEBP.";
            return RedirectToAction(nameof(Details), new { id = destinoId });
        }

        var carpetaUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "usuarios");
        Directory.CreateDirectory(carpetaUploads);

        foreach (var antiguo in Directory.GetFiles(carpetaUploads, $"usuario_{destinoId}.*"))
        {
            System.IO.File.Delete(antiguo);
        }

        var rutaDestino = Path.Combine(carpetaUploads, $"usuario_{destinoId}{ext}");
        await using (var stream = new FileStream(rutaDestino, FileMode.Create))
        {
            await foto.CopyToAsync(stream);
        }

        TempData["Exito"] = "Foto de perfil actualizada correctamente.";
        return RedirectToAction(nameof(Details), new { id = destinoId });
    }
}