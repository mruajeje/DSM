using System.ComponentModel.DataAnnotations;

namespace CarGo.Web.ViewModels;

public sealed class ViajeBuscarViewModel
{
    // Filtro 1: Origen / Destino
    [Display(Name = "Origen")]
    public string? Origen { get; set; }

    [Display(Name = "Destino")]
    public string? Destino { get; set; }

    // Filtro 2: Fecha y Plazas
    [DataType(DataType.DateTime)]
    [Display(Name = "Desde fecha")]
    public DateTime? FechaDesde { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Hasta fecha")]
    public DateTime? FechaHasta { get; set; }

    [Display(Name = "Plazas mínimas")]
    public int? PlazasMinimas { get; set; }

    // Filtro 3: Precio
    [Display(Name = "Precio mínimo")]
    public double? PrecioMinimo { get; set; }

    [Display(Name = "Precio máximo")]
    public double? PrecioMaximo { get; set; }

    // Filtro 4: Preferencias (y estado activo)
    [Display(Name = "Permite mascotas")]
    public bool PermiteMascotas { get; set; }

    [Display(Name = "Permite equipaje")]
    public bool PermiteEquipaje { get; set; }

    // Filtro 5: Viajes disponibles (futuros con plazas)
    [Display(Name = "Solo viajes disponibles (futuros con plazas)")]
    public bool SoloDisponibles { get; set; }

    // Filtro 6: Conductor verificado
    [Display(Name = "Solo conductor verificado")]
    public bool SoloConductorVerificado { get; set; }

    // Resultados transformados por ViajeAssembler
    public List<ViajeViewModel> Resultados { get; set; } = new();
}