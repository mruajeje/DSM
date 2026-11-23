// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(ViajeController_manual_usings) ENABLED START*/
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarGo.Domain.Entities;
using CarGo.Application.Services;
using CarGo.Application.Exceptions;
using CarGo.Web.Assemblers;
using CarGo.Web.ViewModels;
using CarGo.Web.Utils.Security;

using CarGo.Domain.Enums;
using CarGo.Domain.ValueObjects;
/*PROTECTED REGION END*/

namespace CarGo.Web.Controllers;

/*PROTECTED REGION ID(ViajeController_manual_class_attributes) ENABLED START*/
/*PROTECTED REGION END*/
public partial class ViajeController
{
    private async Task<List<ViajeViewModel>> EjecutarBusquedaAsync(ViajeBuscarViewModel filtro)
    {
        IEnumerable<Viaje> resultado = await _viajeService.ReadAllAsync();

        // 1. Filtro Origen / Destino
        if (!string.IsNullOrWhiteSpace(filtro.Origen) || !string.IsNullOrWhiteSpace(filtro.Destino))
        {
            var porRuta = await _viajeService.ReadFilterByOrigenDestinoAsync(
                filtro.Origen?.Trim() ?? string.Empty,
                filtro.Destino?.Trim() ?? string.Empty);
            resultado = resultado.Intersect(porRuta);
        }

        // 2. Filtro Fecha y Plazas
        if (filtro.FechaDesde.HasValue || filtro.FechaHasta.HasValue || (filtro.PlazasMinimas.HasValue && filtro.PlazasMinimas.Value > 0))
        {
            var desde = filtro.FechaDesde ?? new DateTime(2000, 1, 1);
            var hasta = filtro.FechaHasta ?? new DateTime(2100, 1, 1);
            var plazas = filtro.PlazasMinimas ?? 1;
            var porFechaPlazas = await _viajeService.ReadFilterByFechaYPlazasAsync(desde, hasta, plazas);
            resultado = resultado.Intersect(porFechaPlazas);
        }

        // 3. Filtro Precio
        if (filtro.PrecioMinimo.HasValue || filtro.PrecioMaximo.HasValue)
        {
            var min = filtro.PrecioMinimo ?? 0;
            var max = filtro.PrecioMaximo ?? double.MaxValue;
            var porPrecio = await _viajeService.ReadFilterByPrecioAsync(min, max);
            resultado = resultado.Intersect(porPrecio);
        }

        // 4. Filtro Preferencias y Estado
        if (filtro.PermiteMascotas || filtro.PermiteEquipaje)
        {
            var porPreferencias = await _viajeService.ReadFilterByPreferenciasYEstadoAsync(
                filtro.PermiteMascotas,
                filtro.PermiteEquipaje);
            resultado = resultado.Intersect(porPreferencias);
        }

        // 5. Filtro Viajes Disponibles
        if (filtro.SoloDisponibles)
        {
            var disponibles = await _viajeService.ReadFilterViajesDisponiblesAsync(DateTime.Now);
            resultado = resultado.Intersect(disponibles);
        }

        // 6. Filtro Conductor Verificado
        if (filtro.SoloConductorVerificado)
        {
            var verificados = await _viajeService.ReadFilterByConductorVerificadoAsync();
            resultado = resultado.Intersect(verificados);
        }

        return resultado
            .Select(ViajeAssembler.ToViewModel)
            .ToList();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ViajeController_GestionarPlazas_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> GestionarPlazas(int p_oid, int p_nuevasPlazas)
    {
    /*PROTECTED REGION ID(ViajeController_GestionarPlazas_POST_body) ENABLED START*/
        try
        {
            await _viajeService.GestionarPlazasAsync(p_oid, p_nuevasPlazas);
            TempData["SuccessMessage"] = "Plazas actualizadas correctamente.";
        }
        catch (ModelException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al gestionar plazas en viaje {Id}", p_oid);
            TempData["ErrorMessage"] = "No se pudieron actualizar las plazas del viaje.";
        }

        return RedirectToAction(nameof(Details), new { id = p_oid });
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
    public async Task<IActionResult> GeolocalizarTrayecto(int p_oid, string p_latitud, string p_longitud)
    {
    /*PROTECTED REGION ID(ViajeController_GeolocalizarTrayecto_POST_body) ENABLED START*/
        if (!double.TryParse(p_latitud?.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double latitud) ||
            !double.TryParse(p_longitud?.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double longitud))
        {
            TempData["ErrorMessage"] = "Introduce coordenadas numéricas válidas para latitud y longitud.";
            return RedirectToAction(nameof(Details), new { id = p_oid });
        }

        int usuarioId = HttpContext.Session.GetUser<UsuarioViewModel>()?.Id ?? 0;
        if (usuarioId <= 0)
        {
            foreach (var claim in User.Claims)
            {
                if (int.TryParse(claim.Value, out int parsedId) && parsedId > 0)
                {
                    usuarioId = parsedId;
                    break;
                }
            }
        }

        if (usuarioId <= 0)
        {
            TempData["ErrorMessage"] = "No se ha podido identificar al usuario en sesión.";
            return RedirectToAction(nameof(Details), new { id = p_oid });
        }

        try
        {
            await _viajeService.GeolocalizarTrayectoAsync(p_oid, usuarioId, latitud, longitud, DateTime.Now);
            TempData["SuccessMessage"] = "Posición GPS del trayecto actualizada correctamente.";
        }
        catch (ModelException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al geolocalizar el viaje {Id}", p_oid);
            TempData["ErrorMessage"] = "No se pudo actualizar la geolocalización del trayecto.";
        }

        return RedirectToAction(nameof(Details), new { id = p_oid });
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