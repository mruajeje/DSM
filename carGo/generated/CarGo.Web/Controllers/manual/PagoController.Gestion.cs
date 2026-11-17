using Microsoft.AspNetCore.Mvc;
using CarGo.Web.Assemblers;

namespace CarGo.Web.Controllers;

// Acciones propias (pantalla de gestión de pagos). Archivo nuevo: HARIA no lo regenera.
public partial class PagoController
{
    [HttpGet]
    public async Task<IActionResult> Gestionar()
    {
        var pagos = (await _pagoService.ReadAllAsync())
            .Select(PagoAssembler.ToViewModel)
            .OrderByDescending(p => p.FechaPago)
            .ToList();

        return View(pagos);
    }
}
