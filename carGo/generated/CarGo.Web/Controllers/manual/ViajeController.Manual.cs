// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(ViajeController_manual_usings) ENABLED START*/
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
/*PROTECTED REGION ID(ViajeController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION END*/
public partial class ViajeController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ViajeController_GestionarPlazas_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> GestionarPlazas(int p_oid, int p_nuevasPlazas)
    {
    /*PROTECTED REGION ID(ViajeController_GestionarPlazas_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _viajeService.GestionarPlazasAsync(p_oid, p_nuevasPlazas);
        return NoContent();
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ViajeController_GestionarAltaPasajero_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> GestionarAltaPasajero(int p_oid, int p_usuario_oid)
    {
    /*PROTECTED REGION ID(ViajeController_GestionarAltaPasajero_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _viajeService.GestionarAltaPasajeroAsync(p_oid, p_usuario_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ViajeController_GeolocalizarTrayecto_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> GeolocalizarTrayecto(int p_oid, int p_usuario_oid, double p_latitud, double p_longitud, DateTime p_fecha)
    {
    /*PROTECTED REGION ID(ViajeController_GeolocalizarTrayecto_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _viajeService.GeolocalizarTrayectoAsync(p_oid, p_usuario_oid, p_latitud, p_longitud, p_fecha);
        return NoContent();
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ViajeController_GestionarBajaPasajero_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> GestionarBajaPasajero(int p_oid, int p_usuario_oid)
    {
    /*PROTECTED REGION ID(ViajeController_GestionarBajaPasajero_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _viajeService.GestionarBajaPasajeroAsync(p_oid, p_usuario_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }
}
