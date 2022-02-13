using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FinanzasWebApi.Models
{
    public class ReporteEstadoFinanciero
    {
        public int Id { get; set; }

        public string Descripcion { get; set; }

        public virtual ICollection<ElementosDelReporteEF> ElementosDelReporteEF { get; set; }

        

        public ReporteEstadoFinanciero()
        {
            ElementosDelReporteEF = new HashSet<ElementosDelReporteEF>();
        }

        
    }
}