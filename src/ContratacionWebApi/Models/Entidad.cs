using System.ComponentModel.DataAnnotations;

namespace ContratacionWebApi.Models
{
    public class Entidad
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        //todo: preguntar si es solo numeros
        [Required]
        [Display(Name = "Código REUP")]
        public string CodigoReup { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required]
        [RegularExpression("[0-9]*", ErrorMessage = "Solo se admiten números")]
        public string Nit { get; set; }

        [RegularExpression("[0-9,-]*", ErrorMessage = "Solo se admiten números")]
        [Display(Name = "Cuenta bancaria CUC")]
        public string CtaBancariaCuc { get; set; }

        [RegularExpression("[0-9]*", ErrorMessage = "Solo se admiten números")]
        [Display(Name = "Cuenta bancaria MN")]
        public string CtaBancariaMn { get; set; }

        [Display(Name = "Nombre cuenta CUC")]
        public string NombreCtaCuc { get; set; }

        [Display(Name = "Nombre cuenta MN")]
        public string NombreCtaMn { get; set; }

        [Display(Name = "Teléfono")]
        [DataType(DataType.PhoneNumber)]
        public string Telefono { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Fax { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "El dirección de correo no tiene el formato correcto")]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; }
    }
}