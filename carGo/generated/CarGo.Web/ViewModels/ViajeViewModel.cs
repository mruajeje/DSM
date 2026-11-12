using System.ComponentModel.DataAnnotations;

namespace CarGo.Web.ViewModels;

public sealed class ViajeViewModel : IValidatableObject
{
    /*PROTECTED REGION ID(ViajeViewModel_generated) START*/
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [Display(Name = "origen / ciudad")]
    public string OrigenCiudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [Display(Name = "origen / direccion")]
    public string OrigenDireccion { get; set; } = string.Empty;

    [Display(Name = "origen / latitud")]
    public double OrigenLatitud { get; set; }

    [Display(Name = "origen / longitud")]
    public double OrigenLongitud { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [Display(Name = "destino / ciudad")]
    public string DestinoCiudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [Display(Name = "destino / direccion")]
    public string DestinoDireccion { get; set; } = string.Empty;

    [Display(Name = "destino / latitud")]
    public double DestinoLatitud { get; set; }

    [Display(Name = "destino / longitud")]
    public double DestinoLongitud { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime FechaHoraSalida { get; set; }

    public int NumPlazasDisponibles { get; set; }

    [EnumDataType(typeof(CarGo.Domain.Enums.EnumMoneda), ErrorMessage = "El valor de {0} no es válido.")]
    [Display(Name = "precio / moneda")]
    public CarGo.Domain.Enums.EnumMoneda PrecioMoneda { get; set; }

    [Display(Name = "precio / cantidad")]
    public double PrecioCantidad { get; set; }

    public bool PermiteMascotas { get; set; }

    public bool PermiteEquipaje { get; set; }

    [UIHint("HariaReadOnly:Create,Edit")]
    public string? Observaciones { get; set; }

    [EnumDataType(typeof(CarGo.Domain.Enums.EstadoViaje), ErrorMessage = "El valor de {0} no es válido.")]
    public CarGo.Domain.Enums.EstadoViaje Estado { get; set; }

    [UIHint("HariaReadOnly:Create,Edit")]
    public double? LatitudActual { get; set; }

    [UIHint("HariaReadOnly:Create,Edit")]
    public double? LongitudActual { get; set; }

    [UIHint("HariaReadOnly:Create,Edit")]
    [DataType(DataType.DateTime)]
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
    // Add custom properties and validation here.
    private IEnumerable<ValidationResult> ValidateAdditional(ValidationContext validationContext)
    {
        yield break;
    }

    /*PROTECTED REGION END*/
}
