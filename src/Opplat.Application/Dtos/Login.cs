using System.ComponentModel.DataAnnotations;

namespace Opplat.Application.Dtos;

public class Login
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
