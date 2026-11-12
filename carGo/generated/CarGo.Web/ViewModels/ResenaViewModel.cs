using System.ComponentModel.DataAnnotations;

namespace CarGo.Web.ViewModels;

public sealed class ResenaViewModel : IValidatableObject
{
    /*PROTECTED REGION ID(ResenaViewModel_generated) START*/
    [Key]
    public int Id { get; set; }

    public int Puntuacion { get; set; }

    [UIHint("HariaReadOnly:Edit")]
    public string? Comentario { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime FechaCreacion { get; set; }

    [EnumDataType(typeof(CarGo.Domain.Enums.EstadoResena), ErrorMessage = "El valor de {0} no es válido.")]
    public CarGo.Domain.Enums.EstadoResena Estado { get; set; }

    [UIHint("HariaReadOnly:Create,Edit")]
    public string? MotivoModeracion { get; set; }

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
    [Display(Name = "Autor")]
    public int? AutorId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> AutorOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "Autor")]
    public string? AutorLabel { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in ValidateAdditional(validationContext)) yield return result;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ResenaViewModel_manual) ENABLED START*/
    // Add custom properties and validation here.
    private IEnumerable<ValidationResult> ValidateAdditional(ValidationContext validationContext)
    {
        yield break;
    }

    /*PROTECTED REGION END*/
}
