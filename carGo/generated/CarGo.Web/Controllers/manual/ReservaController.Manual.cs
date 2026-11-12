// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(ReservaController_manual_usings) ENABLED START*/
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
/*PROTECTED REGION ID(ReservaController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION END*/
public partial class ReservaController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ReservaController_AceptarPasajero_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> AceptarPasajero(int p_oid, int p_conductor_oid)
    {
    /*PROTECTED REGION ID(ReservaController_AceptarPasajero_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _viajeService.Reserva_aceptarPasajeroAsync(p_oid, p_conductor_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ReservaController_RechazarPasajero_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> RechazarPasajero(int p_oid, int p_conductor_oid)
    {
    /*PROTECTED REGION ID(ReservaController_RechazarPasajero_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _viajeService.Reserva_rechazarPasajeroAsync(p_oid, p_conductor_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ReservaController_CancelarReserva_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> CancelarReserva(int p_oid, int p_usuario_oid)
    {
    /*PROTECTED REGION ID(ReservaController_CancelarReserva_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _viajeService.Reserva_cancelarReservaAsync(p_oid, p_usuario_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }
}
