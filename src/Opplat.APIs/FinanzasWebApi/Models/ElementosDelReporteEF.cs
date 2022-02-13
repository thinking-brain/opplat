using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FinanzasWebApi.Models
{
    public class ElementosDelReporteEF
    {
        public int Id { get; set; }

        public string Descripcion { get; set; }
        public bool ColeccionSubElementos  { get; set; }
        public string Celda { get; set; }
        public string Tipo { get; set; }

        public string Sumar { get; set; }
        public string Restar { get; set; }
        public string Dividir { get; set; }

        public virtual ICollection<SubElementosEfReport> SubElementosEfReports { get; set; }
        public string ReporteEstadoFinancieroDescripcion { get; set; }

        public int ReporteEstadoFinancieroId { get; set; }
        public virtual ReporteEstadoFinanciero ReporteEstadoFinanciero { get; set; }

        public ElementosDelReporteEF()
        {
            SubElementosEfReports = new HashSet<SubElementosEfReport>();
        }

        
    }
}