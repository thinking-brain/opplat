using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace op_costos_api.Models
{
    [Table("Costos_Area")]
    public class Area
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Nombre { get; set; }

        [NotMapped]
        public string Detalles { get { return Codigo + " - " + Nombre; } }
    }
}
