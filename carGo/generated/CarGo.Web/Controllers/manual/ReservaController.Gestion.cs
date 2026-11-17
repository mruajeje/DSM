using Microsoft.AspNetCore.Mvc;
using CarGo.Domain.Enums;
using CarGo.Web.Assemblers;
using CarGo.Web.ViewModels;

namespace CarGo.Web.Controllers;

// Acciones propias (pantallas de gestión y búsqueda). Archivo nuevo: HARIA no lo regenera.
public partial class ReservaController
{
    [HttpGet]
    public async Task<IActionResult> Gestionar(bool mias = false)
    {
        var reservas = (await _viajeService.Reserva_readAllAsync())
            .Select(ReservaAssembler.ToViewModel)
            .ToList();

        // "Mis reservas": solo las que ha hecho el usuario que ha iniciado sesión como pasajero
        if (mias && int.TryParse(User.Identity?.Name, out var usuarioId))
        {
            reservas = reservas.Where(r => r.PasajeroId == usuarioId).ToList();
        }

        ViewData["Mias"] = mias;
        return View(reservas.OrderByDescending(r => r.FechaSolicitud).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Buscar(EstadoReserva? estado, DateTime? desde, DateTime? hasta)
    {
        var modelo = new ReservaBuscarViewModel { Estado = estado, Desde = desde, Hasta = hasta };

        // Sin estado elegido solo se muestra el formulario
        if (estado is null)
        {
            return View(modelo);
        }

        // El filtro del modelo trata "hasta" como exclusivo, así que se suma un día
        // para que la fecha elegida en el formulario cuente como incluida.
        var inicio = (desde ?? DateTime.Today.AddMonths(-1)).Date;
        var fin = (hasta ?? DateTime.Today).Date.AddDays(1);

        if (inicio >= fin)
        {
            ModelState.AddModelError(string.Empty, "La fecha «desde» no puede ser posterior a «hasta».");
            return View(modelo);
        }

        var resultados = await _viajeService.Reserva_readFilterByEstadoYFechaAsync(estado.Value, inicio, fin);
        modelo.Buscado = true;
        modelo.Resultados = resultados.Select(ReservaAssembler.ToViewModel).ToList();
        return View(modelo);
    }
}
