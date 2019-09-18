using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace op_contabilidad_api.ViewModels
{
    public class PronosticoProductivoVM
    {
        public string Obra { get; set; }
        public decimal CUP { get; set; }
        public decimal CUC { get; set; }
        public decimal PlanMes { get; set; }
        public decimal AcumuladoCUP { get; set; }
        public decimal AcumuladoCUC { get; set; }

        [NotMapped]
        public decimal Total { get { return CUP + CUC; } }

        [NotMapped]
        public decimal TotalAcumulado { get { return AcumuladoCUP + AcumuladoCUC; } }
    }
}
