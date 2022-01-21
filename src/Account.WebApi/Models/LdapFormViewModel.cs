using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Account.WebApi.Models
{
    public class LdapFormViewModel
    {
        [Required]
        public string Host { get; set; }
        [DisplayName("Usuario")]
        [Required]
        public string UserName { get; set; }
        [DisplayName("Contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DisplayName("Unidad Organizativa")]
        public string UnidadOrganizativa { get; set; }
        [DataType(DataType.Text)]
        public int Puerto { get; set; }
    }
}