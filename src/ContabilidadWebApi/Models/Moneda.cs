using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContabilidadWebApi.Models
{
    [Table("contb_moneda")]
    public class Moneda
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Sigla { get; set; }
    }
}
