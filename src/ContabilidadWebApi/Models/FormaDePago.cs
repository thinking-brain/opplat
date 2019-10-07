using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContabilidadWebApi.Models
{
    [Table("contb_formas_de_pago")]
    public class FormaDePago
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }
    }
}