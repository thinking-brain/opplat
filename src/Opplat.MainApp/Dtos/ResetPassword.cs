using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Opplat.MainApp.Dtos;
public class ResetPassword
{
    [Required]
    public string UsuarioId { get; set; } = String.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como maximo {1} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Contraseña { get; set; } = String.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    [Compare("Contraseña", ErrorMessage = "La contraseña y la confirmacion no coinciden.")]
    public string ConfirmarContraseña { get; set; } = String.Empty;
}
