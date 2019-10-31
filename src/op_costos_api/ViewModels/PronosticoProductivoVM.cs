using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace op_costos_api.ViewModels
{
    public class PronosticoProductivoVM
    {
        public string Obra { get; set; }
        public decimal CUP { get; set; }
        public decimal CUC { get; set; }
        public decimal PlanMes { get; set; }

        [NotMapped]
        public decimal Total { get { return CUP + CUC; } }
    }
}
