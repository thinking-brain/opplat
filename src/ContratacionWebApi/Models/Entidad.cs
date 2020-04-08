using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ContratacionWebApi.Models {
    public class Entidad {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [Display (Name = "Dirección")]
        public string Direccion { get; set; }

        [Required]
        [RegularExpression ("[0-9]*", ErrorMessage = "Solo se admiten números")]
        public string Nit { get; set; }

        [DataType (DataType.PhoneNumber)]
        public string Fax { get; set; }

        [DataType (DataType.EmailAddress, ErrorMessage = "El dirección de correo no tiene el formato correcto")]
        [Display (Name = "Correo electrónico")]
        public string Correo { get; set; }

        [NotMapped]
        public ICollection<CuentaBancaria> CuentasBancarias { get; set; }
    }
}