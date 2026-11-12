// HARIA-WEB GENERATED sha256:697c5987702218eb236809a2093c9fa991ddf11c889a94fc734e972f796ee5b2
using System.ComponentModel.DataAnnotations;

namespace CarGo.Web.ViewModels;

public sealed class LoginViewModel
{
    public string UserClass { get; set; } = "Usuario";
    public string? Identifier { get; set; }
    [Display(Name = "id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
