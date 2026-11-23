using System.ComponentModel.DataAnnotations;

namespace CarGo.Web.ViewModels;

public sealed class VehiculoViewModel : IValidatableObject
{
    /*PROTECTED REGION ID(VehiculoViewModel_generated) START*/
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [Display(Name = "Marca y modelo")]
    public string MarcaModelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [Display(Name = "Matrícula")]
    public string Matricula { get; set; } = string.Empty;

    [UIHint("HariaReadOnly:Edit")]
    [Display(Name = "Información adicional")]
    public string? InformacionAdicional { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [UIHint("HariaAssociation")]
    [Display(Name = "Propietario")]
    public int? PropietarioId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> PropietarioOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "Propietario")]
    public string? PropietarioLabel { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in ValidateAdditional(validationContext)) yield return result;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(VehiculoViewModel_manual) ENABLED START*/
    private IEnumerable<ValidationResult> ValidateAdditional(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(MarcaModelo) && MarcaModelo.Trim().Length < 2)
        {
            yield return new ValidationResult(
                "La marca y modelo debe tener al menos 2 caracteres.",
                new[] { nameof(MarcaModelo) });
        }

        if (!string.IsNullOrWhiteSpace(Matricula) && Matricula.Trim().Length < 4)
        {
            yield return new ValidationResult(
                "La matrícula debe tener al menos 4 caracteres.",
                new[] { nameof(Matricula) });
        }
    }
    /*PROTECTED REGION END*/
}