using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContabilidadWebApi.VersatModels2
{
    public class FinAsientoAnticipo
    {
        [Key]
        public int Idasiento { get; set; }
        public int documento { get; set; }
        public int receptor { get; set; }
        public int area { get; set; }
        public DateTime? fechaVencimiento { get; set; }
    }
}
