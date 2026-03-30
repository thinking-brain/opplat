using System.ComponentModel.DataAnnotations;
namespace Opplat.Domain.Models;

public class Usuario
{
    [Required]
    public string Nombres { get; set; } = null!;

    [Required]
    public string Apellidos { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public Guid Id { get; set; }

    public bool Activo { get; set; }
}
