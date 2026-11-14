// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(NotificacionController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Application.Exceptions;
using CarGo.Web.Assemblers;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION ID(NotificacionController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION END*/
public partial class NotificacionController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(NotificacionController_MarcarComoLeida_POST_attributes) ENABLED START*/
    [Authorize(Roles = "Pasajero")]
    /*PROTECTED REGION END*/
    public async Task<IActionResult> MarcarComoLeida(int p_oid, int p_usuario_oid)
    {
    /*PROTECTED REGION ID(NotificacionController_MarcarComoLeida_POST_body) ENABLED START*/

        // Solo se puede marcar como leída una notificación propia
        if (!int.TryParse(User.Identity?.Name, out var usuarioId)) return Forbid();

        try
        {
            await _notificacionService.MarcarComoLeidaAsync(p_oid, usuarioId);
        }
        catch (ModelException error)
        {
            TempData["Error"] = error.Message;
        }
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }
}