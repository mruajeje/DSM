using System.ComponentModel.DataAnnotations;

namespace CarGo.Web.ViewModels;

public sealed class ReservaViewModel : IValidatableObject
{
    /*PROTECTED REGION ID(ReservaViewModel_generated) START*/
    [Key]
    public int Id { get; set; }

    public int NumPlazasReservadas { get; set; }

    [EnumDataType(typeof(CarGo.Domain.Enums.EstadoReserva), ErrorMessage = "El valor de {0} no es válido.")]
    public CarGo.Domain.Enums.EstadoReserva Estado { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime FechaSolicitud { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [UIHint("HariaAssociation")]
    [Display(Name = "Viaje")]
    public int? ViajeId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ViajeOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "Viaje")]
    public string? ViajeLabel { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [UIHint("HariaAssociation")]
    [Display(Name = "Pasajero")]
    public int? PasajeroId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> PasajeroOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "Pasajero")]
    public string? PasajeroLabel { get; set; }

    [UIHint("HariaAssociation")]
    [Display(Name = "Pago")]
    public int? PagoId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> PagoOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "Pago")]
    public string? PagoLabel { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in ValidateAdditional(validationContext)) yield return result;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ReservaViewModel_manual) ENABLED START*/
    // Add custom properties and validation here.
    private IEnumerable<ValidationResult> ValidateAdditional(ValidationContext validationContext)
    {
        yield break;
    }

    /*PROTECTED REGION END*/
}
