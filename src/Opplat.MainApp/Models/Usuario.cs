using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Opplat.MainApp.Models;

public class Usuario : IdentityUser
{
    [Required]
    public string Nombres { get; set; } = null!;

    [Required]
    public string Apellidos { get; set; } = null!;

    public bool Activo { get; set; }
}
