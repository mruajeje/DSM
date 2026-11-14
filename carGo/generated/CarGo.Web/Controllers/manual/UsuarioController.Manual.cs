// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(UsuarioController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
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
}