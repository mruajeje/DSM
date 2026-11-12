using System.ComponentModel.DataAnnotations;

namespace CarGo.Web.ViewModels;

public sealed class PagoViewModel : IValidatableObject
{
    /*PROTECTED REGION ID(PagoViewModel_generated) START*/
    [Key]
    public int Id { get; set; }

    [EnumDataType(typeof(CarGo.Domain.Enums.EnumMoneda), ErrorMessage = "El valor de {0} no es válido.")]
    [Display(Name = "monto / moneda")]
    public CarGo.Domain.Enums.EnumMoneda MontoMoneda { get; set; }

    [Display(Name = "monto / cantidad")]
    public double MontoCantidad { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime FechaPago { get; set; }

    [EnumDataType(typeof(CarGo.Domain.Enums.EstadoPago), ErrorMessage = "El valor de {0} no es válido.")]
    public CarGo.Domain.Enums.EstadoPago Estado { get; set; }

    [UIHint("HariaReadOnly:Create,Edit")]
    public string? IdTransaccionPasarela { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [UIHint("HariaAssociation")]
    [Display(Name = "Reserva")]
    public int? ReservaId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ReservaOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "Reserva")]
    public string? ReservaLabel { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in ValidateAdditional(validationContext)) yield return result;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(PagoViewModel_manual) ENABLED START*/
    // Add custom properties and validation here.
    private IEnumerable<ValidationResult> ValidateAdditional(ValidationContext validationContext)
    {
        yield break;
    }

    /*PROTECTED REGION END*/
}
