using System.ComponentModel.DataAnnotations;

namespace Opplat.MainApp.Dtos;
public class Login
{
    [Required]
    public string UserName { get; set; } = String.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = String.Empty;
}
