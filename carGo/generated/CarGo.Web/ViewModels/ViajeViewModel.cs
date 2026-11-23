using System.ComponentModel.DataAnnotations;

namespace CarGo.Web.ViewModels;

public sealed class ViajeViewModel : IValidatableObject
{
    /*PROTECTED REGION ID(ViajeViewModel_generated) START*/
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [Display(Name = "Origen / Ciudad")]
    public string OrigenCiudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [Display(Name = "Origen / Dirección")]
    public string OrigenDireccion { get; set; } = string.Empty;

    [Display(Name = "Origen / Latitud")]
    public double OrigenLatitud { get; set; }

    [Display(Name = "Origen / Longitud")]
    public double OrigenLongitud { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [Display(Name = "Destino / Ciudad")]
    public string DestinoCiudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [Display(Name = "Destino / Dirección")]
    public string DestinoDireccion { get; set; } = string.Empty;

    [Display(Name = "Destino / Latitud")]
    public double DestinoLatitud { get; set; }

    [Display(Name = "Destino / Longitud")]
    public double DestinoLongitud { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Fecha y hora de salida")]
    public DateTime FechaHoraSalida { get; set; }

    [Display(Name = "Plazas disponibles")]
    public int NumPlazasDisponibles { get; set; }

    [EnumDataType(typeof(CarGo.Domain.Enums.EnumMoneda), ErrorMessage = "El valor de {0} no es válido.")]
    [Display(Name = "Moneda")]
    public CarGo.Domain.Enums.EnumMoneda PrecioMoneda { get; set; }

    [Display(Name = "Precio")]
    public double PrecioCantidad { get; set; }

    [Display(Name = "Permite mascotas")]
    public bool PermiteMascotas { get; set; }

    [Display(Name = "Permite equipaje")]
    public bool PermiteEquipaje { get; set; }

    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    [EnumDataType(typeof(CarGo.Domain.Enums.EstadoViaje), ErrorMessage = "El valor de {0} no es válido.")]
    [Display(Name = "Estado del viaje")]
    public CarGo.Domain.Enums.EstadoViaje Estado { get; set; }

    [UIHint("HariaReadOnly:Create,Edit")]
    [Display(Name = "Latitud actual")]
    public double? LatitudActual { get; set; }

    [UIHint("HariaReadOnly:Create,Edit")]
    [Display(Name = "Longitud actual")]
    public double? LongitudActual { get; set; }

    [UIHint("HariaReadOnly:Create,Edit")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Última posición")]
    public DateTime? FechaUltimaPosicion { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [UIHint("HariaAssociation")]
    [Display(Name = "Conductor")]
    public int? ConductorId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ConductorOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "Conductor")]
    public string? ConductorLabel { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in ValidateAdditional(validationContext)) yield return result;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ViajeViewModel_manual) ENABLED START*/
    private IEnumerable<ValidationResult> ValidateAdditional(ValidationContext validationContext)
    {
        if (NumPlazasDisponibles < 0)
        {
            yield return new ValidationResult(
                "El número de plazas disponibles no puede ser negativo.",
                new[] { nameof(NumPlazasDisponibles) });
        }

        if (PrecioCantidad < 0)
        {
            yield return new ValidationResult(
                "El precio no puede ser negativo.",
                new[] { nameof(PrecioCantidad) });
        }
    }
    /*PROTECTED REGION END*/
}