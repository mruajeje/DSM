using CarGo.Domain.Enums;

namespace CarGo.Web.ViewModels;

// Modelo de la pantalla de búsqueda de reservas (no lo genera HARIA).
public sealed class ReservaBuscarViewModel
{
    public EstadoReserva? Estado { get; set; }
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public bool Buscado { get; set; }
    public List<ReservaViewModel> Resultados { get; set; } = new();
}
