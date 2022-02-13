using System.ComponentModel.DataAnnotations;
namespace Opplat.MainApp.Dtos;
public class Register
{
    [Required]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = String.Empty;

    [Required]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = String.Empty;

    [Required]
    [Display(Name = "Nombre de usuario")]
    public string Username { get; set; } = String.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como maximo {1} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Contraseña { get; set; } = String.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    [Compare("Contraseña", ErrorMessage = "La contraseña y la confirmacion no coinciden.")]
    public string ConfirmarContraseña { get; set; } = String.Empty;
}
