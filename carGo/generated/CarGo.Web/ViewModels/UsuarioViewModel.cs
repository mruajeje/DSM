using System.ComponentModel.DataAnnotations;

namespace CarGo.Web.ViewModels;

public sealed class UsuarioViewModel : IValidatableObject
{
    /*PROTECTED REGION ID(UsuarioViewModel_generated) START*/
    [Key]
    public int Id { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    [UIHint("HariaPassword")]
    [System.Text.Json.Serialization.JsonIgnore]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes indicar {0}.")]
    public string Email { get; set; } = string.Empty;

    [UIHint("HariaReadOnly:Create,Edit")]
    public string? Telefono { get; set; }

    [UIHint("HariaReadOnly:Create,Edit")]
    public string? Dni { get; set; }

    public bool EsAdmin { get; set; }

    [EnumDataType(typeof(CarGo.Domain.Enums.EstadoVerificacion), ErrorMessage = "El valor de {0} no es válido.")]
    public CarGo.Domain.Enums.EstadoVerificacion EstadoVerificacion { get; set; }



    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in ValidateAdditional(validationContext)) yield return result;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(UsuarioViewModel_manual) ENABLED START*/
    // Add custom properties and validation here.
    private IEnumerable<ValidationResult> ValidateAdditional(ValidationContext validationContext)
    {
        yield break;
    }

    /*PROTECTED REGION END*/
}
