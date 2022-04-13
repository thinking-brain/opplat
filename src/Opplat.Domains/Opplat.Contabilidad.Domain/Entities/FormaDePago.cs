using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Contabilidad.Domain.Entities;
    [Table("contb_formas_de_pago")]
    public class FormaDePago
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression("[a-z A-Z,ñ,Ñ,í,ó,á,é,ú,Í,Ó,Á,É,Ú, ]*", ErrorMessage = "Solo letras")]
        public string Nombre { get; set; }
    }
