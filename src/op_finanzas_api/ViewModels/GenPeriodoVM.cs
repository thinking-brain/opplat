using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace op_finanzas_api.ViewModels
{
    public  class GenPeriodoVM
    {
        [Key]
        public int Idperiodo { get; set; }
        public byte Idejercicio { get; set; }
        public string Nombre { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public bool? Enuso { get; set; }
    }
}
