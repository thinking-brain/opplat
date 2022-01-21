using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FinanzasWebApi.Models
{
    public class SubElementosEfReport
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string ElementosDelReporteEFDescripcion { get; set; }

        public int ElementosDelReporteEFId { get; set; }
        public virtual ElementosDelReporteEF ElementosDelReporteEF { get; set; }
        
    }
}