using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Opplat.MainApp.ViewModels;

public class LdapFormViewModel
{
    [Required]
    public string Host { get; set; } = String.Empty;

    [DisplayName("Usuario")]
    [Required]
    public string UserName { get; set; } = String.Empty;

    [DisplayName("Contraseña")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = String.Empty;

    [DisplayName("Unidad Organizativa")]
    public string UnidadOrganizativa { get; set; } = String.Empty;

    [DataType(DataType.Text)]
    public int Puerto { get; set; }
}
