using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContabilidadWebApi.Models
{
    public class PlanGI
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        public string Year { get; set; }

        public string Titulo { get; set; }

        public virtual ICollection<DetallePlanGI> Detalles { get; set; }

        public PlanGI()
        {
            Detalles = new HashSet<DetallePlanGI>();
        }
    }
}
