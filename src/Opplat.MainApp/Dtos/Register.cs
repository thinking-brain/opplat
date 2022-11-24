using System.ComponentModel.DataAnnotations;
namespace Opplat.MainApp.Dtos;
public class Register
{
    [Required]
    public string Name { get; set; } = String.Empty;

    [Required]
    public string LastName { get; set; } = String.Empty;

    [Required]
    public string Username { get; set; } = String.Empty;
    
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = String.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como maximo {1} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = String.Empty;

    [DataType(DataType.Password)]
    [Compare("Contraseña", ErrorMessage = "La contraseña y la confirmacion no coinciden.")]
    public string ConfirmPassword { get; set; } = String.Empty;
}
