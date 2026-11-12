// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(ResenaController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Web.Assemblers;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION ID(ResenaController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION END*/
public partial class ResenaController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ResenaController_ModerarContenido_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ModerarContenido(int p_oid, int p_admin_oid, EstadoResena p_decision, string p_motivo)
    {
    /*PROTECTED REGION ID(ResenaController_ModerarContenido_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _resenaService.ModerarContenidoAsync(p_oid, p_admin_oid, p_decision, p_motivo);
        return NoContent();
    /*PROTECTED REGION END*/
    }
}
